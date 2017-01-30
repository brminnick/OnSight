using System;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsPage : ContentPage
	{
		#region Constant Fields
		readonly InspectionDetailsViewModel _viewModel;
		readonly int _inspectionId;
		Button _viewPhotosButton;
		#endregion

		#region Constructors
		public InspectionDetailsPage(int inspectionId)
		{
			_inspectionId = inspectionId;

			_viewModel = new InspectionDetailsViewModel(inspectionId);
			BindingContext = _viewModel;

			var notesEditor = new Editor
			{
				BackgroundColor = Color.Green
			};
			notesEditor.SetBinding(Editor.TextProperty, nameof(_viewModel.TitleText));

			_viewPhotosButton = new Button
			{
				Text = "Photos",
				Image = "MunichREIcon.png",
				BackgroundColor = Color.Pink
			};
			_viewPhotosButton.SetBinding(Button.IsEnabledProperty, nameof(_viewModel.IsPhotosButtonEnabled));

			var relativeLayout = new RelativeLayout();

			Func<RelativeLayout, double> getNotesEditorHeight = (p) => notesEditor.Measure(relativeLayout.Width, relativeLayout.Height).Request.Height;
			Func<RelativeLayout, double> getNotesEditorWidth = (p) => notesEditor.Measure(relativeLayout.Width, relativeLayout.Height).Request.Width;

			Func<RelativeLayout, double> getViewPhotosButtonHeight = (p) => _viewPhotosButton.Measure(relativeLayout.Width, relativeLayout.Height).Request.Height;
			Func<RelativeLayout, double> getViewPhotosButtonWidth = (p) => _viewPhotosButton.Measure(relativeLayout.Width, relativeLayout.Height).Request.Width;

			if (Device.Idiom == TargetIdiom.Phone)
			{
				relativeLayout.Children.Add(notesEditor,
										   Constraint.Constant(0),
										   Constraint.Constant(0),
										   Constraint.RelativeToParent(parent => parent.Width),
										   Constraint.RelativeToParent(parent => parent.Height * 0.4));
			}
			else
			{
				relativeLayout.Children.Add(notesEditor,
										   Constraint.Constant(0),
										   Constraint.Constant(0),
										   Constraint.RelativeToParent(parent => parent.Width),
										   Constraint.RelativeToParent(parent => parent.Height * 0.2));
			}

			relativeLayout.Children.Add(_viewPhotosButton,
			                            Constraint.RelativeToParent(parent => parent.Width / 2 - getViewPhotosButtonWidth(parent) / 2),
									   Constraint.RelativeToParent(parent => parent.Height * 0.4 / 2 - getViewPhotosButtonHeight(parent) / 2));

			this.SetBinding(TitleProperty, nameof(_viewModel.TitleText));

			Padding = new Thickness(20, 10);

			Content = relativeLayout;
		}
		#endregion

		#region Methods

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewPhotosButton.Clicked += HandleViewPhotosButtonClicked;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel?.SaveDataCommand?.Execute(null);

			_viewPhotosButton.Clicked -= HandleViewPhotosButtonClicked;
		}

		void HandleViewPhotosButtonClicked(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new PhotosListPage(_inspectionId)));
		}
		#endregion
	}
}
