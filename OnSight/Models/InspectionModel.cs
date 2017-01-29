using System;
using System.Linq;
using System.Collections.Generic;

using SQLite;

using Xamarin.Forms;

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

		List<NoteModel> InspectionNotesList { get; set; } = new List<NoteModel>();

		List<PhotoModel> InspectionPhotoList { get; set; } = new List<PhotoModel>();

		#endregion

		#region Methods
		public void SavePhoto(PhotoModel photoModel)
		{
			var doesImageNameExist = InspectionPhotoList?.FirstOrDefault(x => x.ImageName.Equals(photoModel.ImageName)) != null;

			if (doesImageNameExist == true)
				InspectionPhotoList.First(x => x.ImageName.Equals(photoModel.ImageName)).ImageStream = photoModel.ImageStream;
			else
				InspectionPhotoList.Add(photoModel);
		}

		public PhotoModel GetPhoto(string imageName)
		{
			return InspectionPhotoList?.FirstOrDefault(x => x.ImageName.Equals(imageName));
		}

		public List<PhotoModel> GetAllPhotos()
		{
			return InspectionPhotoList;
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

		public List<NoteModel> GetAllNotes()
		{
			return InspectionNotesList;
		}
		#endregion
	}
}
