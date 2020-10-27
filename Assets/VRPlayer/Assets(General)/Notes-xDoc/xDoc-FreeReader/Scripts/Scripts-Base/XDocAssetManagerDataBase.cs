using UnityEngine;
using xDocBase.AnnotationTypeModule;

namespace xDocBase.AssetManagement
{

    [System.Serializable]
    public abstract class XDocAssetManagerDataBase : ScriptableObject
    {

        public XDocSettingsBase	settings;
        public XDocAnnotationTypesListBase annotationTypeList;
        public XDocAnnotationTypeBase invalidAnnotationType;
        public XDocWriterBase writer;

    }
}

