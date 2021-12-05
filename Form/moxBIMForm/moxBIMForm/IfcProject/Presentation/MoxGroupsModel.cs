// Funcion Based in IfcGroupsViewModel //
// https://github.com/xBimTeam/XbimWindowsUI/blob/master/Xbim.Presentation/XbimTreeview.cs
// End;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Federation;
using Xbim.Ifc;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;

namespace MoxMain
{

    public class MoxGroupsModel : IXbimViewModel
    {
        private readonly IModel _model;

        private readonly IIfcProject _project;

        private List<IXbimViewModel> _children;

        private bool _isSelected;

        private bool _isExpanded;

        public XbimViewType MoxViewType { get; private set; }

        private string _name;

        public IModel Model => _model;

        public IPersistEntity Entity => null;

        public int EntityLabel => -1;

        public override string ToString()
        {
            return _name;
        }

        public MoxGroupsModel(IModel model, XbimViewType type)
        {
            MoxViewType = type;
            _model = model;
            _name = "Initial Node";
            _project = null;
            IEnumerable subs = Children; //call this once to preload first level of hierarchy   
        }
        
        public MoxGroupsModel(IIfcProject project, XbimViewType type)
        {
            MoxViewType = type;
            _model = project.Model;
            _name = "Initial Node";
            _project = project;
            IEnumerable subs = Children; //call this once to preload first level of hierarchy   
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                Load();
                return _children;
            }
        }

        public IXbimViewModel CreatingParent
        {
            get { return null; }
            set { }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged("IsSelected"); }
        }

        private void Load()
        {
            if (_children == null)
            {
                _children = new List<IXbimViewModel>();

                switch (MoxViewType)
                {
                    case XbimViewType.SpatialStructure:
                        if (_project != null)
                        {
                            foreach (var item in _project.GetSpatialStructuralElements())
                            {
                                _children.Add(new SpatialViewModel(item, this));
                            }

                            var federation = _model as IFederatedModel;
                            if (federation == null) return;

                            foreach (var refModel in federation.ReferencedModels)
                            {
                                _children.Add(new XbimRefModelViewModel(refModel, this));
                            }
                        }
                        break;
                    
                    case XbimViewType.Groups:
                        
                        var allGroups = _model.Instances.OfType<IIfcGroup>();
                        var childGroups = new List<IIfcRoot>();
                        foreach (var obj in _model.Instances.OfType<IIfcRelAssignsToGroup>())
                        {
                            childGroups.AddRange(obj.RelatedObjects.OfType<IIfcGroup>().ToList());
                        }

                        foreach (var item in allGroups)
                        {
                            if (!childGroups.Contains(item))
                                _children.Add(new GroupViewModel(item, this)); //add only root groups/systems
                        }
                        break;

                }
            }
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
        public event PropertyChangedEventHandler PropertyChanged;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }
        void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
