using System;
using System.ComponentModel;

namespace MoxMain
{
	/// <summary>
	/// Custom property class 
	/// </summary>
	public class MoxProperty
	{
		private string sCategory = string.Empty;
		private string sName = string.Empty;
		private bool bReadOnly = false;
		private bool bVisible = true;
		private object objValue = null;

		public MoxProperty(string sCategory, string sName, object value, bool bReadOnly, bool bVisible)
		{
			this.sCategory = sCategory;
			this.sName = sName;
			this.objValue = value;
			this.bReadOnly = bReadOnly;
			this.bVisible = bVisible;
		}

		public bool ReadOnly
		{
			get
			{
				return bReadOnly;
			}
		}

		public string Category
		{
			get
			{
				return sCategory;
			}
		}

		public string Name
		{
			get
			{
				return sName;
			}
		}

		public bool Visible
		{
			get
			{
				return bVisible;
			}
		}

		public object Value
		{
			get
			{
				return objValue;
			}
			set
			{
				objValue = value;
			}
		}

	}

	/// <summary>
	/// Custom PropertyDescriptor
	/// </summary>
	public class MoxPropertyDescriptor : PropertyDescriptor
	{
		MoxProperty m_Property;
		public MoxPropertyDescriptor(ref MoxProperty myProperty, Attribute[] attrs) : base(myProperty.Name, attrs)
		{
			m_Property = myProperty;
		}

		#region PropertyDescriptor specific

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				return null;
			}
		}

		public override object GetValue(object component)
		{
			return m_Property.Value;
		}

		public override string Description
		{
			get
			{
				return m_Property.Name;
			}
		}

		public override string Category
		{
			get
			{
				return m_Property.Category;
			}
		}

		public override string DisplayName
		{
			get
			{
				return m_Property.Name;
			}

		}

		public override bool IsReadOnly
		{
			get
			{
				return m_Property.ReadOnly;
			}
		}

		public override void ResetValue(object component)
		{
			//Have to implement
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void SetValue(object component, object value)
		{
			m_Property.Value = value;
		}

		public override Type PropertyType
		{
			get { return m_Property.Value.GetType(); }
		}

		#endregion
	}
}
