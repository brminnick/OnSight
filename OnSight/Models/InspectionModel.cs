using System;
using System.Linq;
using System.Collections.Generic;

using SQLite;

namespace OnSight
{
	public class InspectionModel
	{
		#region Properties
		[Unique]
		[PrimaryKey]
		[AutoIncrement]
		public int Id { get; set; }

		public string InspectionTitle { get; set; }

		public DateTime InspectionDateUTC { get; set; }

		List<NoteModel> InspectionNotesList { get; set; }

		List<PhotoModel> InspectionImageList { get; set; }

		#endregion

		#region Methods
		public void SavePhoto(PhotoModel photoModel)
		{
			var doesImageNameExist = InspectionImageList?.FirstOrDefault(x => x.ImageName.Equals(photoModel.ImageName)) != null;

			if (doesImageNameExist == true)
				InspectionImageList.First(x => x.ImageName.Equals(photoModel.ImageName)).ImageStream = photoModel.ImageStream;
			else
				InspectionImageList.Add(photoModel);
		}

		public PhotoModel GetPhoto(string imageName)
		{
			return InspectionImageList?.FirstOrDefault(x => x.ImageName.Equals(imageName));
		}

		public void SaveNote(NoteModel noteModel)
		{
			var noteModelFromList = InspectionNotesList?.FirstOrDefault(x => x.Title.Equals(noteModel.Title));

			var doesNoteExist = noteModelFromList != null;

			if (doesNoteExist == true)
				InspectionNotesList.First(x => x.Title.Equals(noteModel.Title)).Details = noteModel.Details;
			else
				InspectionNotesList.Add(noteModel);
		}

		public NoteModel GetNote(string title)
		{
			return InspectionNotesList?.FirstOrDefault(x => x.Title.Equals(title));
		}
		#endregion
	}
}
