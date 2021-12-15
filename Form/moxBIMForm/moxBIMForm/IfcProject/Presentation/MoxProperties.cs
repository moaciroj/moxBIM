// Funcion Based in IfcGroupsViewModel //
// https://github.com/xBimTeam/XbimWindowsUI/blob/master/Xbim.Presentation/IfcMetaDataControl.xaml.cs
// End;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Xbim.Common;
using System.Collections.ObjectModel;
using Xbim.Ifc4.Interfaces;
using Xbim.Common.Metadata;

namespace MoxMain
{
    public class MoxProperties : CollectionBase, ICustomTypeDescriptor
    {
        public class PropertyItem
        {
            public string Units { get; set; }
            public string PropertySetName { get; set; }
            public string Name { get; set; }
            public int IfcLabel { get; set; }
            public string IfcUri
            {
                get { return "xbim://EntityLabel/" + IfcLabel; }
            }
            public bool IsLabel
            {
                get { return IfcLabel > 0; }
            }
            public string Value { get; set; }
            private readonly string[] _schemas = { "file", "ftp", "http", "https" };
            public bool IsLink
            {
                get
                {
                    Uri uri;
                    if (!Uri.TryCreate(Value, UriKind.Absolute, out uri))
                        return false;
                    var schema = uri.Scheme;
                    return _schemas.Contains(schema);
                }
            }
        }

        public void Remove(string Name)
        {
            foreach (MoxProperty prop in base.List)
            {
                if (prop.Name == Name)
                {
                    base.List.Remove(prop);
                    return;
                }
            }
        }

        public void Add(MoxProperty Value)
        {
            base.List.Add(Value);
        }

        public MoxProperty this[int index]
        {
            get
            {
                return (MoxProperty)base.List[index];
            }
            set
            {
                base.List[index] = (MoxProperty)value;
            }
        }

        /*
        public MoxProperties()
        {
            Clear();
        }
        */

        private IPersistEntity _entity;

        private readonly HistoryCollection<IPersistEntity> _history = new HistoryCollection<IPersistEntity>(20);

        private readonly ObservableCollection<PropertyItem> _objectProperties = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> mObjectProperties
        {
            get { return _objectProperties; }
        }

        private readonly ObservableCollection<PropertyItem> _quantities = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> mQuantities
        {
            get { return _quantities; }
        }

        private readonly ObservableCollection<PropertyItem> _properties = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> mProperties
        {
            get { return _properties; }
        }

        private readonly ObservableCollection<PropertyItem> _materials = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> mMaterials
        {
            get { return _materials; }
        }

        private readonly ObservableCollection<PropertyItem> _typeProperties = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> mTypeProperties
        {
            get { return _typeProperties; }
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
                MoxProperty prop = (MoxProperty)this[i];
                newProps[i] = new MoxPropertyDescriptor(ref prop, attributes);
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

        #endregion

        public IPersistEntity Entity
        {
            get { return _entity; }
            set 
            {
                Clear();
                _entity = value;
                FillTabValues();
                SetTabValues(FormMain.SelectedTab);
            }
        }

        private void Clear(bool clearHistory = true)
        {
            _objectProperties.Clear();
            _quantities.Clear();
            _properties.Clear();
            _typeProperties.Clear();
            _materials.Clear();
            if (clearHistory)
                _history.Clear();

            NotifyPropertyChanged("Properties");
            NotifyPropertyChanged("PropertySets");
        }

        public void SetTabValues(selectedTab sel)
        {
            switch (sel)
            {
                case selectedTab.tabMat:
                    Show_properties(_materials);
                    break;
                case selectedTab.tabObj:
                    Show_properties(_objectProperties);
                    break;
                case selectedTab.tabProp:
                    Show_properties(_properties);
                    break;
                case selectedTab.tabQuant:
                    Show_properties(_quantities);
                    break;
                default:
                    Show_properties(_typeProperties);
                    break;
            }
        }

        private void Show_properties (ObservableCollection<PropertyItem> props)
        {
            base.List.Clear();
            for (int i = 0 ; i < props.Count ; i++)
            {
                PropertyItem item = props[i];

                bool _islink = item.IsLink;
                string _category = item.PropertySetName;
                string _name = item.Name;
                string _value = item.Value;
                if (item.IsLabel)
                    _value += " - [IfcLabel: #" + item.IfcLabel + "]";
                MoxProperty p = new MoxProperty(_category, _name, _value, true, true);
                this.Add(p);
            }
        }

        public void FillTabValues()
        {
            //Fill all data
            FillObjectData();
            FillTypeData();
            FillPropertyData();
            FillQuantityData();
            FillMaterialData();
        }

        private void FillMaterialData()
        {
            if (_materials.Any())
                return; //don't fill unless empty

            if (_entity is IIfcObject)
            {
                // add material information
                //
                var ifcObj = _entity as IIfcObject;
                var matRels = ifcObj.HasAssociations.OfType<IIfcRelAssociatesMaterial>();
                foreach (var matRel in matRels)
                {
                    AddMaterialData(matRel.RelatingMaterial, "");
                }
            }
            else if (_entity is IIfcTypeObject)
            {
                var ifcObj = _entity as IIfcTypeObject;
                var matRels = ifcObj.HasAssociations.OfType<IIfcRelAssociatesMaterial>();
                foreach (var matRel in matRels)
                {
                    AddMaterialData(matRel.RelatingMaterial, "");
                }
            }
            if (_entity is IIfcProduct)
            {
                var ifcprod = _entity as IIfcProduct;
                // styles are attached to representation items
                //
                var repItemsLabels = ifcprod.Representation?.Representations?.SelectMany(rep => rep.Items).Select(i => i.EntityLabel).ToList();

                // is there style information too?
                var styles = ifcprod.Model.Instances.OfType<IIfcStyledItem>().Where(x => repItemsLabels != null && (x.Item != null && repItemsLabels.Contains(x.Item.EntityLabel)));
                foreach (var style in styles)
                {
                    int i = 0;
                    _materials.Add(new PropertyItem
                    {
                        PropertySetName = "Connected Style Items",
                        Name = $"Connected Style Items[{i++}]",
                        Value = $"{style.EntityLabel}",
                        IfcLabel = style.EntityLabel
                    });
                }
            }
        }

        private void AddMaterialData(IIfcMaterialSelect matSel, string setName)
        {
            if (matSel is IIfcMaterial) //simplest just add it
                _materials.Add(new PropertyItem
                {
                    Name = $"{((IIfcMaterial)matSel).Name} [#{matSel.EntityLabel}]",
                    PropertySetName = setName,
                    Value = ""
                });
            else if (matSel is IIfcMaterialLayer)
            {
                var asMatLayer = matSel as IIfcMaterialLayer;

                _materials.Add(new PropertyItem
                {
                    Name = $"{asMatLayer.Material?.Name} [#{matSel.EntityLabel}]",
                    Value = asMatLayer.LayerThickness.Value?.ToString(),
                    PropertySetName = setName
                });
            }
            else if (matSel is IIfcMaterialList)
            {
                foreach (var mat in ((IIfcMaterialList)matSel).Materials)
                {
                    _materials.Add(new PropertyItem
                    {
                        Name = $"{mat.Name} [#{mat.EntityLabel}]",
                        PropertySetName = setName,
                        Value = ""
                    });
                }
            }
            else if (matSel is IIfcMaterialLayerSet)
            {
                foreach (var item in ((IIfcMaterialLayerSet)matSel).MaterialLayers) //recursive call to add materials
                {
                    AddMaterialData(item, ((IIfcMaterialLayerSet)matSel).LayerSetName);
                }
            }
            else if (matSel is IIfcMaterialLayerSetUsage)
            {
                //recursive call to add materials
                foreach (var item in ((IIfcMaterialLayerSetUsage)matSel).ForLayerSet.MaterialLayers)
                {
                    AddMaterialData(item, ((IIfcMaterialLayerSetUsage)matSel).ForLayerSet.LayerSetName);
                }
            }
        }

        private void FillPropertyData()
        {
            if (_properties.Any()) //don't try to fill unless empty
                return;
            //now the property sets for any 

            if (_entity is IIfcObject)
            {
                var asIfcObject = (IIfcObject)_entity;
                foreach (
                    var pSet in
                        asIfcObject.IsDefinedBy.Select(
                            relDef => relDef.RelatingPropertyDefinition as IIfcPropertySet)
                    )
                    AddPropertySet(pSet);
            }
            else if (_entity is IIfcTypeObject)
            {
                var asIfcTypeObject = _entity as IIfcTypeObject;
                if (asIfcTypeObject.HasPropertySets == null)
                    return;
                foreach (var pSet in asIfcTypeObject.HasPropertySets.OfType<IIfcPropertySet>())
                {
                    AddPropertySet(pSet);
                }
            }
        }

        private void AddPropertySet(IIfcPropertySet pSet)
        {
            if (pSet == null)
                return;
            foreach (var item in pSet.HasProperties.OfType<IIfcPropertySingleValue>()) //handle IfcPropertySingleValue
            {
                AddProperty(item, pSet.Name);
            }
            foreach (var item in pSet.HasProperties.OfType<IIfcComplexProperty>()) // handle IfcComplexProperty
            {
                // by invoking the undrlying addproperty function with a longer path
                foreach (var composingProperty in item.HasProperties.OfType<IIfcPropertySingleValue>())
                {
                    AddProperty(composingProperty, pSet.Name + " / " + item.Name);
                }
            }
            foreach (var item in pSet.HasProperties.OfType<IIfcPropertyEnumeratedValue>()) // handle IfcComplexProperty
            {
                AddProperty(item, pSet.Name);
            }
        }

        private void AddProperty(IIfcPropertyEnumeratedValue item, string groupName)
        {
            var val = "";
            var nomVals = item.EnumerationValues;
            foreach (var nomVal in nomVals)
            {
                if (nomVal != null)
                    val = nomVal.ToString();
                _properties.Add(new PropertyItem
                {
                    IfcLabel = item.EntityLabel,
                    PropertySetName = groupName,
                    Name = item.Name,
                    Value = val
                });
            }
        }

        private void AddProperty(IIfcPropertySingleValue item, string groupName)
        {
            var val = "";
            var nomVal = item.NominalValue;
            if (nomVal != null)
                val = nomVal.ToString();
            _properties.Add(new PropertyItem
            {
                IfcLabel = item.EntityLabel,
                PropertySetName = groupName,
                Name = item.Name,
                Value = val
            });
        }

        private void FillObjectData()
        {
            if (_objectProperties.Count > 0)
                return; //don't fill unless empty
            if (_entity == null)
                return;

            _objectProperties.Add(new PropertyItem { Name = "Ifc Label", Value = "#" + _entity.EntityLabel, PropertySetName = "General" });

            var ifcType = _entity.ExpressType;
            _objectProperties.Add(new PropertyItem { Name = "Type", Value = ifcType.Type.Name, PropertySetName = "General" });

            var ifcObj = _entity as IIfcObject;
            var typeEntity = ifcObj?.IsTypedBy.FirstOrDefault()?.RelatingType;
            if (typeEntity != null)
            {
                _objectProperties.Add(
                    new PropertyItem
                    {
                        Name = "Defining Type",
                        Value = typeEntity.Name,
                        PropertySetName = "General",
                        IfcLabel = typeEntity.EntityLabel
                    }
                );
            }

            var props = ifcType.Properties.Values;
            foreach (var prop in props)
            {
                ReportProp(_entity, prop, FormMain.ChkVerboseIsChecked);
            }
            var invs = ifcType.Inverses;

            foreach (var inverse in invs)
            {
                ReportProp(_entity, inverse, false);
            }
            //// removed old ui
            //return;
            //var root = _entity as IIfcRoot;
            //if (root == null)
            //    return;            
            //_objectProperties.Add(new PropertyItem {Name = "Name", Value = root.Name, PropertySetName = "OldUI"});
            //_objectProperties.Add(new PropertyItem { Name = "Description", Value = root.Description, PropertySetName = "OldUI" });
            //_objectProperties.Add(new PropertyItem { Name = "GUID", Value = root.GlobalId, PropertySetName = "OldUI" });
            //if (root.OwnerHistory != null)
            //{
            //    var user = root.OwnerHistory.OwningUser?.ToString() ?? "<null>";

            //    var app  = ( root.OwnerHistory.OwningApplication != null
            //        && root.OwnerHistory.OwningApplication.ApplicationIdentifier != null
            //        && !string.IsNullOrEmpty(root.OwnerHistory.OwningApplication.ApplicationIdentifier) ) 
            //        ? root.OwnerHistory.OwningApplication.ApplicationIdentifier.ToString()
            //        : "<null>";


            //    _objectProperties.Add(new PropertyItem
            //    {
            //        Name = "Ownership",
            //        Value = user + " using " + app,
            //        PropertySetName = "OldUI"
            //    });
            //}

            ////now do properties in further specialisations that are text labels
            //foreach (var pInfo in ifcType.Properties.Where
            //    (p => p.Value.EntityAttribute.Order > 4
            //          && p.Value.EntityAttribute.State != EntityAttributeState.DerivedOverride)
            //    ) //skip the first for of root, and derived and things that are objects
            //{
            //    var val = pInfo.Value.PropertyInfo.GetValue(_entity, null);
            //    if (val == null || !(val is ExpressType))
            //        continue;
            //    var pi = new PropertyItem
            //    {
            //        Name = pInfo.Value.PropertyInfo.Name,
            //        Value = ((ExpressType) val).ToString(),
            //        PropertySetName = "OldUI"
            //    };
            //    _objectProperties.Add(pi);
            //}
        }


        private void ReportProp(IPersistEntity entity, ExpressMetaProperty prop, bool verbose)
        {
            object propVal = null;
            try
            {
                propVal = prop.PropertyInfo.GetValue(entity, null);
                if (propVal == null)
                {
                    if (!verbose)
                        return;
                    propVal = "<null>";
                }
            }
            catch (Exception ex)
            {
                var tmpVal = "Error:";
                while (ex != null)
                {
                    tmpVal += " " + ex.Message;
                    ex = ex.InnerException;
                }
                if (prop.EntityAttribute.IsEnumerable)
                    propVal = new[] { tmpVal };
                else
                    propVal = tmpVal;
            }

            if (prop.EntityAttribute.IsEnumerable)
            {
                var propCollection = propVal as System.Collections.IEnumerable;
                if (propCollection != null)
                {
                    var propVals = propCollection.Cast<object>().ToArray();
                    switch (propVals.Length)
                    {
                        case 0:
                            if (!verbose)
                                return;
                            _objectProperties.Add(new PropertyItem { Name = prop.PropertyInfo.Name, Value = "<empty>", PropertySetName = "General" });
                            break;
                        case 1:
                            var tmpSingle = GetPropItem(propVals[0]);
                            tmpSingle.Name = prop.PropertyInfo.Name + "[0]";
                            tmpSingle.PropertySetName = "General";
                            _objectProperties.Add(tmpSingle);
                            break;
                        default:
                            int i = 0;
                            foreach (var item in propVals)
                            {
                                var tmpLoop = GetPropItem(item);
                                tmpLoop.Name = $"{prop.PropertyInfo.Name}[{i++}]";
                                tmpLoop.PropertySetName = prop.PropertyInfo.Name;
                                _objectProperties.Add(tmpLoop);
                            }
                            break;
                    }
                }
                else
                {
                    if (!verbose)
                        return;
                    _objectProperties.Add(new PropertyItem { Name = prop.PropertyInfo.Name, Value = "<not an enumerable>" });
                }
            }
            else
            {
                var tmp = GetPropItem(propVal);
                tmp.Name = prop.PropertyInfo.Name;
                tmp.PropertySetName = "General";
                _objectProperties.Add(tmp);
            }
        }


        private PropertyItem GetPropItem(object propVal)
        {
            var retItem = new PropertyItem();

            var pe = propVal as IPersistEntity;
            var propLabel = 0;
            if (pe != null)
            {
                propLabel = pe.EntityLabel;
            }
            var ret = propVal.ToString();
            if (ret == propVal.GetType().FullName)
            {
                ret = propVal.GetType().Name;
            }

            if (pe is IIfcRepresentation)
            {
                var t = (IIfcRepresentation)pe;
                ret += $" ('{t.RepresentationIdentifier}' '{t.RepresentationType}')";
            }
            else if (pe is IIfcRelDefinesByProperties)
            {
                var t = (IIfcRelDefinesByProperties)pe;
                var stringValues = new List<string>();
                var name = t.RelatingPropertyDefinition?.PropertySetDefinitions.FirstOrDefault()?.Name;
                if (!string.IsNullOrEmpty(name))
                    stringValues.Add($"'{name}'");
                if (stringValues.Any())
                {
                    ret += $" ({string.Join(" ", stringValues.ToArray())})";
                }
            }
            else if (pe is IIfcRoot)
            {
                var t = (IIfcRoot)pe;
                var stringValues = new List<string>();
                if (!string.IsNullOrEmpty(t.Name))
                    stringValues.Add($"'{t.Name}'");
                if (!string.IsNullOrEmpty(t.GlobalId))
                    stringValues.Add($"'{t.GlobalId}'");
                if (stringValues.Any())
                {
                    ret += $" ({string.Join(" ", stringValues.ToArray())})";
                }
            }
            else if (pe is IIfcOwnerHistory)
            {
                var t = (IIfcOwnerHistory)pe;
                var stringValues = new List<string>();
                if (!string.IsNullOrEmpty(t.OwningUser?.EntityLabel.ToString()))
                    stringValues.Add($"#{t.OwningUser?.EntityLabel.ToString()}");
                if (!string.IsNullOrEmpty(t.OwningApplication?.ApplicationIdentifier))
                    stringValues.Add($"'{t.OwningApplication?.ApplicationIdentifier}'");
                if (stringValues.Any())
                {
                    ret += $" ({string.Join(" using ", stringValues.ToArray())})";
                }
            }
            retItem.Value = ret;
            retItem.IfcLabel = propLabel;
            return retItem;
        }

        private void FillTypeData()
        {
            if (_typeProperties.Count > 0)
                return; // only fill once
            var ifcObj = _entity as IIfcObject;
            var typeEntity = ifcObj?.IsTypedBy.FirstOrDefault()?.RelatingType;
            if (typeEntity == null)
                return;
            var ifcType = typeEntity?.ExpressType;

            _typeProperties.Add(new PropertyItem { Name = "Type", Value = ifcType.Type.Name });
            _typeProperties.Add(new PropertyItem { Name = "Ifc Label", Value = "#" + typeEntity.EntityLabel });

            _typeProperties.Add(new PropertyItem { Name = "Name", Value = typeEntity.Name });
            _typeProperties.Add(new PropertyItem { Name = "Description", Value = typeEntity.Description });
            _typeProperties.Add(new PropertyItem { Name = "GUID", Value = typeEntity.GlobalId });
            if (typeEntity.OwnerHistory != null)
            {
                _typeProperties.Add(new PropertyItem
                {
                    Name = "Ownership",
                    Value =
                       typeEntity.OwnerHistory.OwningUser + " using " +
                       typeEntity.OwnerHistory.OwningApplication.ApplicationIdentifier
                });
            }
            //now do properties in further specialisations that are text labels
            foreach (var pInfo in ifcType.Properties.Where
                (p => p.Value.EntityAttribute.Order > 4
                      && p.Value.EntityAttribute.State != EntityAttributeState.DerivedOverride)
                ) //skip the first for of root, and derived and things that are objects
            {
                var val = pInfo.Value.PropertyInfo.GetValue(typeEntity, null);
                if (!(val is ExpressType))
                    continue;
                var pi = new PropertyItem { Name = pInfo.Value.PropertyInfo.Name, Value = ((ExpressType)val).ToString() };
                _typeProperties.Add(pi);
            }
        }

        private void FillQuantityData()
        {
            if (_quantities.Count > 0) return; //don't fill unless empty
            //now the property sets for any 

            // local cache for efficiency

            var o = _entity as IIfcObject;
            if (o != null)
            {
                var ifcObj = o;
                var modelUnits = _entity.Model.Instances.OfType<IIfcUnitAssignment>().FirstOrDefault();
                // not optional, should never return void in valid model

                foreach (
                    var relDef in
                        ifcObj.IsDefinedBy.Where(r => r.RelatingPropertyDefinition is IIfcElementQuantity))
                {
                    var pSet = relDef.RelatingPropertyDefinition as IIfcElementQuantity;
                    AddQuantityPSet(pSet, modelUnits);
                }
            }
            else if (_entity is IIfcTypeObject)
            {
                var asIfcTypeObject = _entity as IIfcTypeObject;
                var modelUnits = _entity.Model.Instances.OfType<IIfcUnitAssignment>().FirstOrDefault();
                // not optional, should never return void in valid model

                if (asIfcTypeObject.HasPropertySets == null)
                    return;
                foreach (var pSet in asIfcTypeObject.HasPropertySets.OfType<IIfcElementQuantity>())
                {
                    AddQuantityPSet(pSet, modelUnits);
                }

                //foreach (var relDef in ifcObj. IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition is IfcElementQuantity))
                //{
                //    var pSet = relDef.RelatingPropertyDefinition as IfcElementQuantity;
                //    AddQuantityPSet(pSet, modelUnits);
                //}
            }
        }

        private void AddQuantityPSet(IIfcElementQuantity pSet, IIfcUnitAssignment modelUnits)
        {
            if (pSet == null)
                return;
            if (modelUnits == null) throw new ArgumentNullException(nameof(modelUnits));
            foreach (var item in pSet.Quantities.OfType<IIfcPhysicalSimpleQuantity>())
            // currently only handles IfcPhysicalSimpleQuantity
            {
                _quantities.Add(new PropertyItem
                {
                    PropertySetName = pSet.Name,
                    Name = item.Name,
                    Value = GetValueString(item, modelUnits)
                });
            }
        }


        private static string GetUnit(IIfcUnitAssignment units, IfcUnitEnum type)
        {
            var unit = units?.Units.OfType<IIfcNamedUnit>().FirstOrDefault(u => u.UnitType == type);
            return unit?.FullName;
        }

        private static string GetValueString(IIfcPhysicalSimpleQuantity quantity, IIfcUnitAssignment modelUnits)
        {
            if (quantity == null)
                return "";
            string value = null;
            var unitName = "";
            var u = quantity.Unit;
            if (quantity.Unit != null)
                unitName = quantity.Unit.FullName;

            var length = quantity as IIfcQuantityLength;
            if (length != null)
            {
                value = length.LengthValue.ToString();
                if (quantity.Unit == null)
                    unitName = GetUnit(modelUnits, IfcUnitEnum.LENGTHUNIT);
            }
            var area = quantity as IIfcQuantityArea;
            if (area != null)
            {
                value = area.AreaValue.ToString();
                if (quantity.Unit == null)
                    unitName = GetUnit(modelUnits, IfcUnitEnum.AREAUNIT);
            }
            var weight = quantity as IIfcQuantityWeight;
            if (weight != null)
            {
                value = weight.WeightValue.ToString();
                if (quantity.Unit == null)
                    unitName = GetUnit(modelUnits, IfcUnitEnum.MASSUNIT);
            }
            var time = quantity as IIfcQuantityTime;
            if (time != null)
            {
                value = time.TimeValue.ToString();
                if (quantity.Unit == null)
                    unitName = GetUnit(modelUnits, IfcUnitEnum.TIMEUNIT);
            }
            var volume = quantity as IIfcQuantityVolume;
            if (volume != null)
            {
                value = volume.VolumeValue.ToString();
                if (quantity.Unit == null)
                    unitName = GetUnit(modelUnits, IfcUnitEnum.VOLUMEUNIT);
            }
            var count = quantity as IIfcQuantityCount;
            if (count != null)
                value = count.CountValue.ToString();


            if (string.IsNullOrWhiteSpace(value))
                return "";

            return string.IsNullOrWhiteSpace(unitName) ?
                value :
                $"{value} {unitName}";
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion

    }


}
