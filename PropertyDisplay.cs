// http://www.atmarkit.co.jp/fdotnet/dotnettips/291pgridjapan/pgridjapan.html
using System;
using System.ComponentModel;

namespace PSUTools
{
	/// <summary>
	/// プロパティ表示名を外部から設定するための属性。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	class PropertyDisplayNameAttribute : Attribute
	{
		private string myPropertyDisplayName;

		public PropertyDisplayNameAttribute(string name) : base()
		{
			myPropertyDisplayName = name;
		}

		public string PropertyDisplayName
		{
			get
			{
				return myPropertyDisplayName;
			}
		}
	}

	/// <summary>
	/// プロパティ表示名でPropertyDisplayPropertyDescriptorクラスを使用するために
	/// TypeConverter属性に指定するためのTypeConverter派生クラス。
	/// </summary>
	public class PropertyDisplayConverter : TypeConverter
	{
		public PropertyDisplayConverter()
		{
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object instance, Attribute[] filters)
		{
			PropertyDescriptorCollection collection = new PropertyDescriptorCollection(null);

			PropertyDescriptorCollection properies = TypeDescriptor.GetProperties(instance, filters, true);
			foreach(PropertyDescriptor desc in properies)
			{
				collection.Add(new PropertyDisplayPropertyDescriptor(desc));
			}

			return collection;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}

	/// <summary>
	/// プロパティの説明（＝情報）を提供するクラス。DisplayNameをカスタマイズする。
	/// </summary>
	public class PropertyDisplayPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor oneProperty;

		public PropertyDisplayPropertyDescriptor(PropertyDescriptor desc) : base(desc)
		{
			oneProperty = desc;
		}

		public override bool CanResetValue(object component)
		{
			return oneProperty.CanResetValue(component);
		}

		public override Type ComponentType
		{
			get
			{
				return oneProperty.ComponentType;
			}
		}

		public override object GetValue(object component)
		{
			return oneProperty.GetValue(component);
		}

		public override string Description
		{
			get
			{
				return oneProperty.Description;
			}
		}

		public override string Category
		{
			get
			{
				return oneProperty.Category;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return oneProperty.IsReadOnly;
			}
		}

		public override void ResetValue(object component)
		{
			oneProperty.ResetValue(component);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return oneProperty.ShouldSerializeValue(component);
		}

		public override void SetValue(object component, object value)
		{
			oneProperty.SetValue(component, value);
		}

		public override Type PropertyType
		{
			get
			{
				return oneProperty.PropertyType;
			}
		}

		public override string DisplayName
		{
			get
			{
				PropertyDisplayNameAttribute attrib =
					(PropertyDisplayNameAttribute)oneProperty.Attributes[typeof(PropertyDisplayNameAttribute)];
				if (attrib != null)
				{
					return attrib.PropertyDisplayName;
				}

				return oneProperty.DisplayName;
			}
		}

	}

}
