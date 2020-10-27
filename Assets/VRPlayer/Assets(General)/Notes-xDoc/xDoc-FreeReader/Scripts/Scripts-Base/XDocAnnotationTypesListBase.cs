using System.Collections.Generic;
using UnityEngine;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public abstract class XDocAnnotationTypesListBase : ScriptableObject
	{
	
		[SerializeField]
		public List<XDocAnnotationTypeBase> annotationTypesList;

		void OnEnable()
		{
			if (annotationTypesList == null) {
				annotationTypesList = new List<XDocAnnotationTypeBase>();
			} 
		}

	}
}
