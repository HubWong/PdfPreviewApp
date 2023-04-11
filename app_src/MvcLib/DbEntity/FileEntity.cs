using MvcLib.Db;
using MvcLib.DbEntity.MainContent;
using MvcLib.Sidebar;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcLib.DbEntity
{
    /// <summary>
    /// if form file needed.
    /// </summary>
    public interface IUploadModel
    {
        /// <summary>
        /// update file path in db
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        string AddorUpdatePath(string path);
    }

    public class FileEntity : BasicData
    {
        public string path { get; set; }
        public long length { get; set; }
        public string fk_id { get; set; }
        public DateTime make_day { get; set; }
        public string maker { get; set;}
    }

    /// <summary>
    /// file repository interface
    /// </summary>
    public interface IFileRepos : 
        IDataBase<FileEntity>,
        IDbQuery<FileEntity>,
        IUploadModel
    {

    }

}
