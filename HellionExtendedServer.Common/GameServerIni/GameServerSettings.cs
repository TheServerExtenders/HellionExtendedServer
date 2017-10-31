// Referenced From https://stackoverflow.com/a/314025

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace HellionExtendedServer.Common.GameServerIni
{
    /// <summary>
    /// CustomClass (Which is binding to property grid)
    /// </summary>
    public class GameServerSettings : CollectionBase, ICustomTypeDescriptor
    {
        /// <summary>
        /// Add CustomProperty to CollectionBase List
        /// </summary>
        /// <param name="Value"></param>
        public void Add(GameServerProperty Value)
        {
            base.List.Add(Value);
        }

        /// <summary>
        /// Remove item from List
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Name)
        {
            foreach (GameServerProperty prop in base.List)
            {
                if (prop.Name == Name)
                {
                    base.List.Remove(prop);
                    return;
                }
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public GameServerProperty this[int index]
        {
            get
            {
                return (GameServerProperty)base.List[index];
            }
            set
            {
                base.List[index] = (GameServerProperty)value;
            }
        }

        #region "TypeDescriptor Implementation"

        /// <summary>
        /// Get Class Name
        /// </summary>
        /// <returns>String</returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <returns>AttributeCollection</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// GetComponentName
        /// </summary>
        /// <returns>String</returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// GetConverter
        /// </summary>
        /// <returns>TypeConverter</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// GetDefaultEvent
        /// </summary>
        /// <returns>EventDescriptor</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// GetDefaultProperty
        /// </summary>
        /// <returns>PropertyDescriptor</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// GetEditor
        /// </summary>
        /// <param name="editorBaseType">editorBaseType</param>
        /// <returns>object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] newProps = new PropertyDescriptor[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                GameServerProperty prop = (GameServerProperty)this[i];
                newProps[i] = new GameServerPropertyDescriptor(ref prop, attributes);
            }

            return new PropertyDescriptorCollection(newProps);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion "TypeDescriptor Implementation"
    }

    /// <summary>
    /// Custom property class
    /// </summary>
    public class GameServerProperty
    {
        private string sName = string.Empty;
        private string sDisplayName = string.Empty;
        private bool bReadOnly = false;
        private bool bVisible = true;
        private string sDescription = string.Empty;
        private string sCategory = string.Empty;
        private object objValue = null;
        private Setting setting = new Setting();

        public GameServerProperty()
        {
        }

        public GameServerProperty(string sName, string sDisplayName, string sCategory, string sDescription, object value, Type type, bool bReadOnly, bool bVisible)
        {
            this.sName = sName;
            this.sDisplayName = sDisplayName;
            this.sCategory = sCategory;
            this.objValue = value;
            this.type = type;
            this.bReadOnly = bReadOnly;
            this.bVisible = bVisible;
            this.sDescription = sDescription;
        }

        private Type type;

        public Type Type
        {
            get { return type; }
        }

        public bool ReadOnly
        {
            get
            {
                return bReadOnly;
            }
        }

        public string Name
        {
            get
            {
                return sName;
            }
        }

        public string DisplayName
        {
            get
            {
                return sDisplayName;
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

        public string Description
        {
            get
            {
                return sDescription;
            }
            set
            {
                sDescription = value;
            }
        }

        public string Category
        {
            get
            {
                return sCategory;
            }
            set
            {
                sCategory = value;
            }
        }

        public Setting Setting
        {
            get
            {
                return setting;
            }
            set
            {
                setting = value;
            }
        }

        public Setting GetAsSetting()
        {

            setting.Value = Value;

           

            return setting;
        }

        public GameServerProperty SetFromSetting(Setting value)
        {

            sName = value.Name;
            sDisplayName = new CultureInfo("en-US").TextInfo.ToTitleCase(value.Name.Replace("_", " ")) + " (Default: " + value.DefaultValue + ")";
            sCategory = value.Category;
            sDescription = value.Description;
            type = value.Type;
            objValue = value.Value;            

            this.setting = value;

            return this;
        }
    }

    /// <summary>
    /// Custom PropertyDescriptor
    /// </summary>
    public class GameServerPropertyDescriptor : PropertyDescriptor
    {
        private GameServerProperty m_Property;

        public GameServerPropertyDescriptor(ref GameServerProperty myProperty, Attribute[] attrs) : base(myProperty.Name, attrs)
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
            get { return m_Property.Type; }
        }

        public override object GetValue(object component)
        {
            return m_Property.Value;
        }

        public override string Description
        {
            get { return m_Property.Description; }
        }

        public override string Category
        {
            get { return m_Property.Category; }
        }

        public override string DisplayName
        {
            get { return m_Property.DisplayName; }
        }

        public override bool IsReadOnly
        {
            get { return m_Property.ReadOnly; }
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
            get { return m_Property.Type; }
        }

        #endregion PropertyDescriptor specific
    }
}