﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Core.Presentation;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Presentation.Services;
using System.Activities.Tracking;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Activities.XamlIntegration;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Windows.Input;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Reflection;
using System.Linq;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Presentation.Factories;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.Windows.Media;
using static System.Activities.Presentation.WorkflowDesignerIcons;

namespace RPA_Slayer
{
    public partial class wfDesigner : UserControl
    {

        public WorkflowDesigner WorkflowDesigner;
        public IDesignerDebugView DebuggerService;

        public const String DefultWorkflowFilePath = @"..\..\DefaultWorkflows\defaultWorkflow.xaml";
        public  string WorkflowFilePath = DefultWorkflowFilePath;

        TextBox logsTxtbox;
        TextBox outputTxtbox;

        Dictionary<int, SourceLocation> textLineToSourceLocationMap;
        Dictionary<object, SourceLocation> designerSourceLocationMapping = new Dictionary<object, SourceLocation>();
        Dictionary<object, SourceLocation> wfElementToSourceLocationMap = null;

        AutoResetEvent resumeRuntimeFromHost = null;
        List<SourceLocation> breakpointList = new List<SourceLocation>();



        public wfDesigner()
        {
            InitializeComponent();
            this.RegisterMetadata();
            this.AddWorkflowDesigner();
            this.AddToolBox();
            this.AddTrackingTextbox();
            this.AddOutputTextbox();
        }

        private void RegisterMetadata()
        {
            (new DesignerMetadata()).Register();
        }
        public void AddWorkflowDesigner()
        {
            this.WorkflowDesigner = new WorkflowDesigner();

            this.WorkflowDesigner.Context.Services.GetService<DesignerConfigurationService>().AutoConnectEnabled = true;

            this.WorkflowDesigner.Context.Services.GetService<DesignerConfigurationService>().PanModeEnabled = true;
            this.WorkflowDesigner.Context.Services.GetService<DesignerConfigurationService>().MultipleItemsDragDropEnabled = true;
            this.WorkflowDesigner.Context.Services.GetService<DesignerConfigurationService>().LoadingFromUntrustedSourceEnabled = true;
            this.WorkflowDesigner.Context.Services.GetService<DesignerConfigurationService>().TargetFrameworkName = new System.Runtime.Versioning.FrameworkName(".NETFramework", new Version(4, 5));
            this.DebuggerService = this.WorkflowDesigner.DebugManagerView; 
            if (!string.IsNullOrEmpty(WorkflowFilePath))
            {

                this.WorkflowDesigner.Load(WorkflowFilePath);

            }

            //nevermind

            this.workflowDesignerPanel.Content = this.WorkflowDesigner.View;
            this.AddPropertyInspector();
            if (WorkflowFilePath == @"..\..\DefaultWorkflows\defaultWorkflow.xaml")
            {
                // Do nothing
            }
            else
            {
                this.SetupFileExplorer();

            }


            //Updating the mapping between Model item and Source Location as soon as you load the designer so that BP setting can re-use that information from the DesignerSourceLocationMapping.
            wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService();


        }
        public  string CutStringAtLastBackslash(string input)
        {
            int lastBackslashIndex = input.LastIndexOf('\\');
            if (lastBackslashIndex >= 0)
            {
                return input.Substring(0, lastBackslashIndex + 1);
            }
            else
            {
                return input;
            }
        }



        private List<string> loadedAssemblyFiles = new List<string>();
     



    private ToolboxControl GetToolboxControl()
        {

            Console.WriteLine("loadedAss:");
            Console.WriteLine(string.Join(Environment.NewLine, loadedAssemblyFiles));

            // Load assemblies
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Activities\Assemblies");

            string[] assemblyFiles = Directory.GetFiles(directoryPath, "*.dll");

            foreach (string assemblyFile in assemblyFiles)
            {
                try
                {
                    string targetFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(assemblyFile));

                    File.Copy(assemblyFile, targetFilePath, true);

                    string logFilePath = @"..\..\DeletedItems.txt"; // Replace with the actual path of the log file

                    try
                    {
                        // Read all lines from the log file
                        string[] lines = File.ReadAllLines(logFilePath);

                        // Iterate over each line and combine it with a path, then add it to the loadedAssemblyFiles list
                        foreach (string line in lines)
                        {
                            string path = Path.Combine(@"..\..\Activities\Assemblies", line); // Replace directoryPath1 with the desired directory path
                            loadedAssemblyFiles.Add(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading log file: " + ex.Message);
                    }



                    if (!loadedAssemblyFiles.Contains(assemblyFile))
                    {
                        loadedAssemblyFiles.Add(assemblyFile);
                        AssemblyName assemblyName = AssemblyName.GetAssemblyName(targetFilePath);
                        Assembly.LoadFrom(targetFilePath);
                    }
                   


                    // Optionally, you can perform further processing with the loaded assembly if needed
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading assembly '{assemblyFile}': {ex.Message}");
                }
            }

            var toolboxControl = new ToolboxControl();
            var appAssemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.GetName().Name);
            int activitiesCount = 0;

            foreach (var activityLibrary in appAssemblies)
            {
                var wfToolboxCategory = new ToolboxCategory(activityLibrary.GetName().Name);
                var selectedActivities = activityLibrary.GetExportedTypes()
                    .Where(activityType => activityType.IsVisible
                        && activityType.IsPublic
                        && !activityType.IsNested
                        && !activityType.IsAbstract
                        && activityType.GetConstructor(Type.EmptyTypes) != null
                        && !activityType.Name.Contains('`')
                        && (activityType.IsSubclassOf(typeof(Activity))
                            || activityType.IsSubclassOf(typeof(NativeActivity))
                            || activityType.IsSubclassOf(typeof(DynamicActivity))
                            || activityType.IsSubclassOf(typeof(ActivityWithResult))
                            || activityType.IsSubclassOf(typeof(AsyncCodeActivity))
                            || activityType.IsSubclassOf(typeof(CodeActivity))
                            || activityType == typeof(System.Activities.Statements.FlowNode)
                            || activityType == typeof(System.Activities.Statements.State)
                            || activityType == typeof(System.Activities.Core.Presentation.FinalState)
                            || activityType == typeof(System.Activities.Statements.FlowDecision)
                            || activityType == typeof(System.Activities.Statements.FlowStep)
                            || activityType == typeof(System.Activities.Statements.FlowSwitch<Type>)
                            || activityType == typeof(System.Activities.Statements.Switch<Type>)
                            || activityType == typeof(System.Activities.Statements.TryCatch)
                            || activityType == typeof(System.Activities.Statements.While)))
                    .OrderBy(activityType => activityType.Name)
                    .Select(activityType => new ToolboxItemWrapper(activityType));

                foreach (var toolboxItemWrapper in selectedActivities)
                {
                    wfToolboxCategory.Add(toolboxItemWrapper);
                }

                if (wfToolboxCategory.Tools.Count > 0)
                {
                    toolboxControl.Categories.Add(wfToolboxCategory);
                    activitiesCount += wfToolboxCategory.Tools.Count;
                }
            }
            //fixed ForEach
            toolboxControl.Categories.Add(
                   new System.Activities.Presentation.Toolbox.ToolboxCategory
                   {
                       CategoryName = "CustomForEach",
                       Tools = {
                                new ToolboxItemWrapper(typeof(System.Activities.Core.Presentation.Factories.ForEachWithBodyFactory<>)),
                                new ToolboxItemWrapper(typeof(System.Activities.Core.Presentation.Factories.ParallelForEachWithBodyFactory<>))
                       }
                   }
            );

            return toolboxControl;
        }

        public void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            this.toolboxPanel.Content = tc;
        }
        public void RemoveToolBox()
        {
            this.toolboxPanel.Content = null;
        }
        private void AddPropertyInspector()
        {
            if (this.WorkflowDesigner == null)
                return;

            this.WorkflowPropertyPanel.Content = this.WorkflowDesigner.PropertyInspectorView;
        }
        

        private void SetupFileExplorer()
        {
            var root = new TreeViewItem();
            root.Header = "Explorer";
            root.Tag = new DirectoryInfo(CutStringAtLastBackslash(WorkflowFilePath)); // Set the root directory here

            PopulateTreeView(root);
            WorkflowFileExplorer.Content = root;
        }

        private void PopulateTreeView(TreeViewItem item)
        {
            var directory = item.Tag as DirectoryInfo;
            if (directory == null)
            {
                return;
            }

            try
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    var subItem = new TreeViewItem();
                    subItem.Header = subDirectory.Name;
                    subItem.Tag = subDirectory;

                    subItem.Items.Add("*"); // Placeholder item to allow expansion

                    subItem.Expanded += (s, e) =>
                    {
                        if (subItem.Items.Count == 1 && subItem.Items[0] is string)
                        {
                            subItem.Items.Clear();

                            try
                            {
                                foreach (var subSubDirectory in subDirectory.GetDirectories())
                                {
                                    var subSubItem = new TreeViewItem();
                                    subSubItem.Header = subSubDirectory.Name;
                                    subSubItem.Tag = subSubDirectory;

                                    subSubItem.Items.Add("*"); // Placeholder item to allow expansion

                                    subSubItem.Expanded += (s1, e1) =>
                                    {
                                        if (subSubItem.Items.Count == 1 && subSubItem.Items[0] is string)
                                        {
                                            subSubItem.Items.Clear();

                                            try
                                            {
                                                foreach (var file in subSubDirectory.GetFiles())
                                                {
                                                    var fileItem = new TreeViewItem();
                                                    fileItem.Header = file.Name;
                                                    fileItem.Tag = file;

                                                    subSubItem.Items.Add(fileItem);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                // Handle exception
                                            }
                                        }   
                                    };

                                    subItem.Items.Add(subSubItem);
                                }

                                foreach (var file in subDirectory.GetFiles())
                                {
                                    var fileItem = new TreeViewItem();
                                    fileItem.Header = file.Name;
                                    fileItem.Tag = file;

                                    subItem.Items.Add(fileItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle exception
                            }
                        }
                    };

                    item.Items.Add(subItem);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        private void TabItem_GotFocus_RefreshXamlBox(object sender, RoutedEventArgs e)
        {

            this.WorkflowDesigner.Flush(); //save the current state of the workflow to the Test() property
            xamlTextBox.Text = this.WorkflowDesigner.Text;

        }
        #region New-Open-save - Run - Stop - Debug - Clear

        public void SaveWorkflow()
        {
            if (WorkflowDesigner != null && (WorkflowFilePath != DefultWorkflowFilePath))
            {
                WorkflowDesigner.Flush();
                WorkflowDesigner.Save(WorkflowFilePath);
            }
            else
            {
                string dummyFileName = "New_WF";
                SaveFileDialog sf = new SaveFileDialog();
                // Feed the dummy name to the save dialog
                sf.Filter = "XAML files(.xaml)|*.xaml";
                sf.FilterIndex = 2;
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() == true)
                {
                    // Now here's our save folder
                    string savePath = System.IO.Path.GetDirectoryName(sf.FileName);
                    Debug.WriteLine(savePath);
                    Debug.WriteLine(sf.FileName);
                    // Do whatever
                    WorkflowFilePath = sf.FileName;
                    WorkflowDesigner.Flush();
                    WorkflowDesigner.Save(WorkflowFilePath);


                }
            }
        }

        public void NewWorkflow()
        {
            ClearWorkflow();

            WorkflowFilePath = DefultWorkflowFilePath;

            AddWorkflowDesigner();
        }

        public void OpenWorkflow()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "XAML files(.xaml)|*.xaml";

            if (dlg.ShowDialog() == true)
            {
                WorkflowFilePath = dlg.FileName;
                ClearWorkflow();
                AddWorkflowDesigner();
            }
        }

        public void FastRunWorkflow()
        {
            if (!string.IsNullOrEmpty(WorkflowFilePath))
            {
                try
                {
                    SaveWorkflow();
                    var workflow = ActivityXamlServices.Load(WorkflowFilePath);

                    // Create a SimulatorTrackingParticipant and register it with the workflow application
                    var trackingParticipant = new SimulatorTrackingParticipant
                    {
                        ActivityIdToWorkflowElementMap = new Dictionary<string, Activity>()
                    };

                    WorkflowApplication application = new WorkflowApplication(workflow);
                    application.Extensions.Add(trackingParticipant);

                    // Register a callback for the TrackingRecordReceived event to print activity logs
                    trackingParticipant.TrackingRecordReceived += (sender, e) =>
                    {
                        var record = e.Record as ActivityStateRecord;

                        if (record != null && record.Activity != null)
                        {
                            Console.WriteLine($"Activity {record.Activity.Name} ({record.Activity.Id}) is in state {record.State}");
                        }
                    };

                    // Run the workflow application
                    application.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }




        #endregion

            #region debuging Runner 

        public void RunWorkflow()
        {

            WorkflowDesigner.Flush();
            SaveWorkflow();
            logsTxtbox.Text = String.Empty;
            outputTxtbox.Text = String.Empty;
            AddWorkflowDesigner();
            this.logsName.Focus();

            WorkflowInvoker instance = new WorkflowInvoker(GetRuntimeExecutionRoot());
            resumeRuntimeFromHost = new AutoResetEvent(false);

            Dictionary<string, Activity> activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);

            #region Set up the Custom Tracking
            const String all = "*";
            SimulatorTrackingParticipant simTracker = new SimulatorTrackingParticipant()
            {
                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries =
                    {
                        new CustomTrackingQuery()
                        {
                            Name = all,
                            ActivityName = all
                        },
                        new WorkflowInstanceQuery()
                        {
                            // Limit workflow instance tracking records for started and completed workflow states
                            States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed, WorkflowInstanceStates.Idle },
                        },
                        new ActivityStateQuery()
                        {
                            // Subscribe for track records from all activities for all states
                            ActivityName = all,
                            States = { all },

                            // Extract workflow variables and arguments as a part of the activity tracking record
                            // VariableName = "*" allows for extraction of all variables in the scope
                            // of the activity
                            Variables =
                            {
                                { all }
                            }
                        }
                    }
                }
            };

            simTracker.ActivityIdToWorkflowElementMap = activityIdToWfElementMap;

            #endregion

            //As the tracking events are received
            simTracker.TrackingRecordReceived += (trackingParticpant, trackingEventArgs) =>
            {
                if (trackingEventArgs.Activity != null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        String.Format("<+=+=+=+> Activity Tracking Record Received for ActivityId: {0}, record: {1} ",
                        trackingEventArgs.Activity.Id,
                        trackingEventArgs.Record
                        
                        )
                    );
                    try
                    {

                        ShowDebug(wfElementToSourceLocationMap[trackingEventArgs.Activity]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("eeee");
                    }
                    this.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() =>
                    {
                        //Textbox Updates
                        logsTxtbox.AppendText(trackingEventArgs.Activity.Id + " " + trackingEventArgs.Activity.DisplayName + " " + ((ActivityStateRecord)trackingEventArgs.Record).State + "\n");
                        logsTxtbox.AppendText("Instance ID: " + ((ActivityStateRecord)trackingEventArgs.Record).InstanceId + "\n");
                        logsTxtbox.AppendText("Activity: " + ((ActivityStateRecord)trackingEventArgs.Record).Activity.Name + "\n");
                        logsTxtbox.AppendText("Level: " + ((ActivityStateRecord)trackingEventArgs.Record).Level + "\n");
                        logsTxtbox.AppendText("Time: " + ((ActivityStateRecord)trackingEventArgs.Record).EventTime + "\n");
                        logsTxtbox.AppendText("************************\n");
                        //if activity is write line  "add output:{content}" to logsTxtbox
                        if (trackingEventArgs.Activity is System.Activities.Statements.WriteLine &&
               ((ActivityStateRecord)trackingEventArgs.Record).State == ActivityStates.Executing)
                        {
                            var writeLineActivity = (System.Activities.Statements.WriteLine)trackingEventArgs.Activity;
                            var outputContent = writeLineActivity.Text;

                            // Check if the output content is an InArgument
                            if (outputContent is System.Activities.InArgument<string> inArgument)
                            {
                                var expression = inArgument.Expression;
                                var value = WorkflowInvoker.Invoke(expression);

                                outputTxtbox.AppendText(((ActivityStateRecord)trackingEventArgs.Record).EventTime+": "  + value + "\n");
                            }
                            else
                            {
                                outputTxtbox.AppendText("Output: " + outputContent + "\n");

                            }
                        }

                        if (trackingEventArgs.Activity.DisplayName == "Log" &&
                         ((ActivityStateRecord)trackingEventArgs.Record).State == ActivityStates.Closed){
                            //Get OutputText from activity 
                            
                                
                        }

                        //Add a sleep so that the debug adornments are visible to the user
                        System.Threading.Thread.Sleep(100); 
                    }));
                }
            };

            instance.Extensions.Add(simTracker);
            ThreadPool.QueueUserWorkItem(new WaitCallback((context) =>
            {
                //Start the Runtime
                instance.Invoke();// new TimeSpan(1,0,0));

                //This is to remove the final debug adornment
                this.Dispatcher.Invoke(DispatcherPriority.Render
                    , (Action)(() =>
                    {
                        this.WorkflowDesigner.DebugManagerView.CurrentLocation = new SourceLocation(WorkflowFilePath, 1, 1, 1, 10);
                    }));

            }));


        }


        #endregion

        #region Helper Methods----(ClearWorkflow-BuildActivityIdToWfElementMap- UpdateSourceLocationMappingInDebuggerService-GetRootInstanc..................)
        public void ClearWorkflow()
        {
            if (WorkflowDesigner != null)
            {
                grid1.Children.Remove(WorkflowDesigner.View);
                grid1.Children.Remove(WorkflowDesigner.PropertyInspectorView);
                WorkflowDesigner = null;
            }
        }
        void ShowDebug(SourceLocation srcLoc)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
                {

                    this.WorkflowDesigner.DebugManagerView.CurrentLocation = srcLoc;

                }));

            //Check if this is where any Breakpoint is set
            bool isBreakpointHit = false;
            foreach (SourceLocation src in breakpointList)
            {
                if (src.StartLine == srcLoc.StartLine && src.EndLine == srcLoc.EndLine)
                {
                    isBreakpointHit = true;
                }
            }

            if (isBreakpointHit == true)
            {
                resumeRuntimeFromHost.WaitOne();
            }

        }



        private Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;

        }


        Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService()
        {
            object rootInstance = GetRootInstance();
            Dictionary<object, SourceLocation> sourceLocationMapping = new Dictionary<object, SourceLocation>();


            if (rootInstance != null)
            {
                Activity documentRootElement = GetRootWorkflowElement(rootInstance);

                SourceLocationProvider.CollectMapping(GetRootRuntimeWorkflowElement(), documentRootElement, sourceLocationMapping,
                    this.WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

                //Collect the mapping between the Model Item tree and its underlying source location
                SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping,
                   this.WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

            }

            // Notify the DebuggerService of the new sourceLocationMapping.
            // When rootInstance == null, it'll just reset the mapping.
            //DebuggerService debuggerService = debuggerService as DebuggerService;
            if (this.DebuggerService != null)
            {
                ((DebuggerService)this.DebuggerService).UpdateSourceLocations(designerSourceLocationMapping);
            }

            return sourceLocationMapping;
        }


        object GetRootInstance()
        {
            ModelService modelService = this.WorkflowDesigner.Context.Services.GetService<ModelService>();
            if (modelService != null)
            {
                return modelService.Root.GetCurrentValue();
            }
            else
            {
                return null;
            }
        }


        // Get root WorkflowElement.  Currently only handle when the object is ActivitySchemaType or WorkflowElement.
        // May return null if it does not know how to get the root activity.
        Activity GetRootWorkflowElement(object rootModelObject)
        {
            System.Diagnostics.Debug.Assert(rootModelObject != null, "Cannot pass null as rootModelObject");

            Activity rootWorkflowElement;
            IDebuggableWorkflowTree debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
            if (debuggableWorkflowTree != null)
            {
                rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
            }
            else // Loose xaml case.
            {
                rootWorkflowElement = rootModelObject as Activity;
            }
            return rootWorkflowElement;
        }

        Activity GetRuntimeExecutionRoot()
        {

            Activity root = ActivityXamlServices.Load(WorkflowFilePath);
            WorkflowInspectionServices.CacheMetadata(root);

            return root;
        }


        Activity GetRootRuntimeWorkflowElement()
        {

            Activity root = ActivityXamlServices.Load(WorkflowFilePath);
            WorkflowInspectionServices.CacheMetadata(root);


            IEnumerator<Activity> enumerator1 = WorkflowInspectionServices.GetActivities(root).GetEnumerator();
            //Get the first child of the x:class
            enumerator1.MoveNext();
            root = enumerator1.Current;
            return root;
        }
        void AddTrackingTextbox()
        {
            logsTxtbox = new TextBox
            {
                IsReadOnly = true,
                Foreground = Brushes.Gray,
                BorderThickness = new Thickness(0.3)


            };
            ;
            Grid.SetRow(logsTxtbox, 1);
            this.TrackingRecord.Children.Add(logsTxtbox);
            
            textLineToSourceLocationMap = new Dictionary<int, SourceLocation>();
        }

        void AddOutputTextbox()
        {
            outputTxtbox = new TextBox
            {
                IsReadOnly = true,
                Foreground = Brushes.Gray,
                BorderThickness = new Thickness(0.3)


            };
            ;
            Grid.SetRow(outputTxtbox, 1);
            this.output.Children.Add(outputTxtbox);
        }

        #endregion

        #region Debugging controls


        public void BreakPointToggle()
        {

            ModelItem mi = this.WorkflowDesigner.Context.Items.GetValue<Selection>().PrimarySelection;
            Activity a = mi.GetCurrentValue() as Activity;
            Console.WriteLine(a);
            

            if (a != null)
            {
                SourceLocation srcLoc = designerSourceLocationMapping[a];
                if (!breakpointList.Contains(srcLoc))
                {
                    this.WorkflowDesigner.Context.Services.GetService<IDesignerDebugView>().UpdateBreakpoint(srcLoc, BreakpointTypes.Bounded | BreakpointTypes.Enabled);
                    breakpointList.Add(srcLoc);
                }
                else
                {
                    this.WorkflowDesigner.Context.Services.GetService<IDesignerDebugView>().UpdateBreakpoint(srcLoc, BreakpointTypes.None);
                    breakpointList.Remove(srcLoc);
                }
            }


        }

        public void _continue()
        {
            resumeRuntimeFromHost.Set();
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key == Key.F9)
            {
                BreakPointToggle();
            }
            else if (e.Key == Key.F5)
            {
                _continue();
            }
            
        }




        #endregion


    }
}
//+=+=+=+> Activity Tracking Record Received for ActivityId: 1.29, record: ActivityStateRecord { InstanceId = 7fba487e-b6a2-4cea-8e5d-8f7564f736c2, RecordNumber = 19, EventTime = 2/4/2023 8:21:12 AM, Activity { Name=Eating Cookie ( 5 seconds), ActivityId = 1.29, ActivityInstanceId = 9, TypeName=System.Activities.Statements.Delay }, State = Closed } 
//Cookie was Good
//<+=+=+=+> Activity Tracking Record Received for ActivityId: 1.27, record: ActivityStateRecord { InstanceId = 7fba487e-b6a2-4cea-8e5d-8f7564f736c2, RecordNumber = 20, EventTime = 2/4/2023 8:21:12 AM, Activity { Name=End, ActivityId = 1.27, ActivityInstanceId = 10, TypeName=System.Activities.Statements.WriteLine }, State = Executing } 
