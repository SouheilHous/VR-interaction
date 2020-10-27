using System.Collections.Generic;
using xDocBase.CustomData;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public class AnnotationTypeData : AnnotationTypeAttributeSetBase
	{
		public bool showDataArea;
		public bool	useObjectReferencesList;
		/// <summary>
		/// <para>
		/// The data field list. This is the list shown to the user to be edited and used when
		/// the annotation type is applied to the annotation. Changes are not
		/// applied immediately to existing annotations, as these changes are not mere 
		/// style changes, which could easily be reverted.
		/// </para>
		/// <para>
		/// Changes on these "master data" will have impact on the instance data of existing annotations:
		/// additinal data fields, removed data fields and thus lost data, changed data fields (name or type and
		/// thus lost data, bc. data content may not be saved due to type incompatibility).
		/// </para>
		/// <para>
		/// We cant initiate this data migration/change all the time during the edit phase. Once the user
		/// is done with editing, he has to apply them and the changes will take effect: 
		/// the migration of existing annotations
		/// will be initiated
		/// </para>
		/// </summary>
		public List<DataField>	dataFieldList;

		public AnnotationTypeData(
			XDocAnnotationTypeBase parent
		)
			: base(
				"Data",
				parent
			)
		{
		}

		public override void Init()
		{
			showDataArea = false;
			useObjectReferencesList = false;
			dataFieldList = new List<DataField>();
		}
		
	}
}

