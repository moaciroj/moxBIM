// Funcion Based in IfcGroupsViewModel //
// https://github.com/xBimTeam/XbimWindowsUI/blob/master/Xbim.Presentation/IfcMetaDataControl.xaml.cs
// End;

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Xbim.Ifc4.Interfaces;
using Xbim.Common.Metadata;

namespace MoxMain
{
    public partial class MoxProperties : PropertyGrid
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

        private IPersistEntity _entity;
        

        private readonly HistoryCollection<IPersistEntity> _history = new HistoryCollection<IPersistEntity>(20);

        private readonly ObservableCollection<PropertyItem> _objectProperties = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> ObjectProperties
        {
            get { return _objectProperties; }
        }

        private readonly ObservableCollection<PropertyItem> _quantities = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> Quantities
        {
            get { return _quantities; }
        }

        private readonly ObservableCollection<PropertyItem> _properties = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> Properties
        {
            get { return _properties; }
        }

        private readonly ObservableCollection<PropertyItem> _materials = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> Materials
        {
            get { return _materials; }
        }

        private readonly ObservableCollection<PropertyItem> _typeProperties = new ObservableCollection<PropertyItem>();

        public ObservableCollection<PropertyItem> TypeProperties
        {
            get { return _typeProperties; }
        }

        public MoxProperties()
        {
            InitializeComponent();
        }


        public MoxProperties(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
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
