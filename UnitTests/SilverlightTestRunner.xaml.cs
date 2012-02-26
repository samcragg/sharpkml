using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Threading;
using System.Reflection;
using NUnit.Framework;

namespace UnitTests
{
    /// <summary>Represents the main visual of the application.</summary>
    public partial class SilverlightTestRunner : Page
    {
        /// <summary>Identifies the FailedTests dependency property.</summary>
        public static readonly DependencyProperty FailedTestsProperty =
            DependencyProperty.Register("FailedTests", typeof(int), typeof(SilverlightTestRunner), null);

        /// <summary>Identifies the PassedTests dependency property.</summary>
        public static readonly DependencyProperty PassedTestsProperty =
            DependencyProperty.Register("PassedTests", typeof(int), typeof(SilverlightTestRunner), null);

        private readonly Thread testRunnerThread;

        /// <summary>
        /// Initializes a new instance of the SilverlightTestRunner class.
        /// </summary>
        public SilverlightTestRunner()
        {
            this.InitializeComponent();

            this.testRunnerThread = new Thread(this.TestRunner);
            this.testRunnerThread.IsBackground = true;
            this.testRunnerThread.Start();
        }

        /// <summary>Gets or sets the number of tests that have failed.</summary>
        public int FailedTests
        {
            get { return (int)GetValue(FailedTestsProperty); }
            set { SetValue(FailedTestsProperty, value); }
        }

        /// <summary>Gets or sets the number of tests that have passed.</summary>
        public int PassedTests
        {
            get { return (int)GetValue(PassedTestsProperty); }
            set { SetValue(PassedTestsProperty, value); }
        }

        private void TestRunner()
        {
            this.AppendLine("Starting tests...");
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                RunTestFixture(type);
            }
            this.AppendLine("Finished.");
        }

        private void RunTestFixture(Type type)
        {
            if (Attribute.IsDefined(type, typeof(TestFixtureAttribute)))
            {
                AppendLine(type.Name);
                object instance = Activator.CreateInstance(type);
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    RunTest(method, instance);
                }
            }
        }

        private void RunTest(MethodInfo method, object instance)
        {
            if (Attribute.IsDefined(method, typeof(TestAttribute)))
            {
                AppendLine("    " + method.Name + " : ");
                try
                {
                    method.Invoke(instance, null);

                    // If we made it here then it passed!
                    this.Dispatcher.BeginInvoke(() => this.PassedTests += 1);
                    ModifyLastLine("PASSED", Colors.Green);
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException.GetType() == typeof(InconclusiveException))
                    {
                        ModifyLastLine("SKIPPED", Colors.Orange);
                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(() => this.FailedTests += 1);
                        ModifyLastLine("FAILED", Colors.Red);
                        AppendLine("        " + ex.InnerException.GetType());
                        AppendLine("        " + ex.InnerException.Message);
                    }
                }
            }
        }

        private void AppendLine(string text)
        {
            this.Dispatcher.BeginInvoke(delegate
            {
                var run = new Run();
                run.Text = text;

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(run);
                this.output.Blocks.Add(paragraph);
                this.output.Selection.Select(paragraph.ContentEnd, paragraph.ContentEnd);
            });
        }

        private void ModifyLastLine(string text, Color color)
        {
            this.Dispatcher.BeginInvoke(delegate
            {
                var paragraph = (Paragraph)this.output.Blocks[this.output.Blocks.Count - 1];
                var run = (Run)paragraph.Inlines[0];

                run.Foreground = new SolidColorBrush(color);
                run.Text += text;
            });
        }
    }
}
