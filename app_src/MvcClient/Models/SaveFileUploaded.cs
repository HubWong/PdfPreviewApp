using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MvcClient.Models
{
    public class FileDbModel
    {
        /// <summary>
        /// db model base
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <param name="success"></param>
        public FileDbModel(string id, string path, bool success)
        {
            Id = id;
            Path = path;
            Success = success;
        }

        public string Id { get; }
        public string Path { get; set; }
        public bool Success { get; set; }
    }

    /// <summary>
    /// just save the file and update the db data.
    /// </summary>
    public class SaveFileUploaded
    {
        /// <summary>
        /// result
        /// </summary>
        public List<FileDbModel> fileDbModels { get; private set; }
        public string SavingFolder { get; set; }
        public List<IFormFile> FormFiles { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savingFolder"></param>
        /// <param name="formFiles"></param>
        /// <param name="saveFunc">maybe update many .</param>
        public SaveFileUploaded(string savingFolder,
            List<IFormFile> formFiles,
            Func<string, string> saveFunc)
        {
            fileDbModels = fileDbModels;
            SavingFolder = savingFolder;
            FormFiles = formFiles;
            this.saveFunc = saveFunc;
        }

        private readonly Func<string, string> saveFunc;
        public bool IsAdd
        {
            get
            {
                return this.fileDbModels == null;
            }
        }
        public void CreateDir()
        {
            if (!Directory.Exists(SavingFolder))
            {
                Directory.CreateDirectory(SavingFolder);
            }
        }


        private IFormFile cur_form_file { get; set; }

        private string cur_file_path_name
        {
            get
            {
                return Path.Combine(SavingFolder,
                    Path.GetRandomFileName() + Path.GetExtension(cur_form_file.FileName));
            }
        }

        private Task save_to_disk()
        {
            using (var stream = File.Create(cur_file_path_name))
            {
                return cur_form_file.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// save the data to db or update it. need a method with the signature.
        /// <path,saved_id>  
        /// </summary>
        Func<string, string> AddorUpdateDb;

        /// <summary>
        /// update the filedbmodel with new data.
        /// </summary>
        /// <returns></returns>
        public Task Save()
        {
            if (IsAdd)
            {
                this.fileDbModels = new List<FileDbModel>();
            }

            if (AddorUpdateDb == null)
            {
                throw new Exception("no fun  defined");
            }

            CreateDir();
            string id;
            foreach (var file in FormFiles)
            {
                cur_form_file = file;
                save_to_disk();
                id = AddorUpdateDb(this.cur_file_path_name);
                fileDbModels.Add(new FileDbModel(id, this.cur_file_path_name, true));
            }
            return Task.CompletedTask;
        }
    }


}
