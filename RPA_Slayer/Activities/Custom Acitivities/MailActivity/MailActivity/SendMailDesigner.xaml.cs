using MailActivity;
using System;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MailActivity
{
    // Custom activity designer for SendMail activity
    public partial class SendMailDesigner : UserControl
    {
        public SendMailDesigner()
        {
            InitializeComponent();
        }
    }

    // Attribute to associate the custom designer with the SendMail activity
    public sealed class SendMailDesignerAttribute : Attribute, IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();

            // Register the custom designer for SendMail activity
            builder.AddCustomAttributes(
                typeof(SendMail),
                new DesignerAttribute(typeof(SendMailDesigner)));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
