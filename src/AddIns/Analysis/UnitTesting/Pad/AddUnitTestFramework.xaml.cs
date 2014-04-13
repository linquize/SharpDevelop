using System;
using System.Linq;
using System.Windows;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Interaction logic for AddUnitTestFramework.xaml
	/// </summary>
	public partial class AddUnitTestFramework : Window
	{
		public TestFrameworkDescriptor SelectedTestFramework { get; private set; }
		public TestFrameworkReferenceSource SelectedReferenceSource { get; private set; }
		
		public AddUnitTestFramework()
		{
			InitializeComponent();
			ITestService testService = SD.GetRequiredService<ITestService>();
			UnitTestFramework.ItemsSource = testService.TestFrameworks;
			if (testService.TestFrameworks.Any())
				UnitTestFramework.SelectedIndex = 0;
			ReferenceSource.ItemsSource = Enum.GetValues(typeof(TestFrameworkReferenceSource));
			ReferenceSource.SelectedItem = TestFrameworkReferenceSource.AssumeInstalled;
		}

		void okButtonClick(object sender, RoutedEventArgs e)
		{
			SelectedTestFramework = (TestFrameworkDescriptor)UnitTestFramework.SelectedItem;
			SelectedReferenceSource = (TestFrameworkReferenceSource)ReferenceSource.SelectedItem;
			DialogResult = true;
			this.Close();
		}
	}
}