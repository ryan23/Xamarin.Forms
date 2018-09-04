using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_ImageRenderer))]
	public class Image : View, IImageController, IElementConfiguration<Image>, IViewController
	{
		public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(ImageSource), typeof(Image), default(ImageSource),
			propertyChanging: ImageElementManager.ImageSourceChanging, propertyChanged: ImageElementManager.ImageSourceChanged);

		public static readonly BindableProperty AspectProperty = BindableProperty.Create("Aspect", typeof(Aspect), typeof(Image), Aspect.AspectFit);

		public static readonly BindableProperty IsOpaqueProperty = BindableProperty.Create("IsOpaque", typeof(bool), typeof(Image), false);

		internal static readonly BindablePropertyKey IsLoadingPropertyKey = BindableProperty.CreateReadOnly("IsLoading", typeof(bool), typeof(Image), default(bool));

		public static readonly BindableProperty IsLoadingProperty = IsLoadingPropertyKey.BindableProperty;


		event EventHandler<BindableValueChangedEventArgs> _imageSourceChanged;
		event EventHandler<BindableValueChangedEventArgs> _imageSourceChanging;
		event EventHandler _imageSourcesSourceChanged;
		readonly Lazy<PlatformConfigurationRegistry<Image>> _platformConfigurationRegistry;

		public Image()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Image>>(() => new PlatformConfigurationRegistry<Image>(this));
			ImageElementManager.Init(this);
		}

		public Aspect Aspect
		{
			get { return (Aspect)GetValue(AspectProperty); }
			set { SetValue(AspectProperty, value); }
		}

		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
		}

		public bool IsOpaque
		{
			get { return (bool)GetValue(IsOpaqueProperty); }
			set { SetValue(IsOpaqueProperty, value); }
		}

		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		protected override void OnBindingContextChanged()
		{
			ImageElementManager.OnBindingContextChanged(this, this);
			base.OnBindingContextChanged();
		}

		[Obsolete("OnSizeRequest is obsolete as of version 2.2.0. Please use OnMeasure instead.")]
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			SizeRequest desiredSize = base.OnSizeRequest(double.PositiveInfinity, double.PositiveInfinity);
			return ImageElementManager.Measure(this, desiredSize, widthConstraint, heightConstraint);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetIsLoading(bool isLoading)
		{
			SetValue(IsLoadingPropertyKey, isLoading);
		}

		public IPlatformElementConfiguration<T, Image> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		BindableProperty IImageController.SourceProperty => SourceProperty;
		BindableProperty IImageController.AspectProperty => AspectProperty;
		BindableProperty IImageController.IsOpaqueProperty => IsOpaqueProperty;

		//event EventHandler<BindableValueChangedEventArgs> IImageController.ImageSourceChanged
		//{
		//	add => _imageSourceChanged += value;
		//	remove => _imageSourceChanged -= value;
		//}

		//event EventHandler<BindableValueChangedEventArgs> IImageController.ImageSourceChanging
		//{
		//	add => _imageSourceChanging += value;
		//	remove => _imageSourceChanging -= value;
		//}

		//event EventHandler IImageController.ImageSourcesSourceChanged
		//{
		//	add => _imageSourcesSourceChanged += value;
		//	remove => _imageSourcesSourceChanged -= value;
		//}

		private void OnSourceChanged(object sender, EventArgs e) =>
			_imageSourcesSourceChanged?.Invoke(this, EventArgs.Empty);

		static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			var image = ((Image)bindable);

			if (newvalue != null)
			{
				((ImageSource)newvalue).SourceChanged += image.OnSourceChanged;
			}
			image._imageSourceChanged?.Invoke(bindable, new BindableValueChangedEventArgs(bindable, oldvalue, newvalue));

		}

		static void OnSourcePropertyChanging(BindableObject bindable, object oldvalue, object newvalue)
		{
			var image = ((Image)bindable);
			if (oldvalue != null)
			{
				((ImageSource)oldvalue).SourceChanged -= image.OnSourceChanged;
			}

			image._imageSourceChanging?.Invoke(bindable, new BindableValueChangedEventArgs(bindable, oldvalue, newvalue));
		}

		void IImageController.RaiseImageSourcePropertyChanged() => OnPropertyChanged(nameof(Source));
	}
}