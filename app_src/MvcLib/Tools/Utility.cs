using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvcLib.Tools
{
    public static class Utility
    {
        public static string EnumExtension(this Enum enumValue)
        {
            return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName();
        }
           

         public static Task<long> FileSize(string path)
        {
            long x = 0;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
               x= new FileInfo(path).Length;                
            }
            return Task.FromResult(x);
        }

        /// <summary>      
        /// 2021-10-13 13:22 interupted. To start new job of sscms
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetDisplayName(Type type) 
        {            
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var p in type.GetProperties())
            {
                DisplayAttribute dp = p.GetCustomAttribute<DisplayAttribute>();
                if (!string.IsNullOrEmpty(dp?.Name))
                {                
                    dic.Add(p.Name, dp.Name.ToString());
                }
                
            }
            return dic;
        }

        /// <summary>
        /// get display name by type
        /// </summary>
        /// <typeparam name="T">meta type</typeparam>
        /// <param name="data"></param>
        /// <param name="propNames"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetDisplayName<T>(T data,string[] propNames)
        {
            var type =data.GetType();
            var dic = new Dictionary<string, string>();
            foreach (string item in propNames)
            {
                dic.Add(item,type.GetProperty(item).GetValue(data, null).ToString());
            }          
            return dic;
        }

        /// <summary>
        /// get index of enum by display name in the type of enum
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static int EnumGetDisplayIndex<T>(string displayName) 
                   where T:Enum
        {
            int intRst = -1;
            try
            {
                IEnumerable<T> arr = Enum.GetValues(typeof(T)).Cast<T>(); 
                for (int i = 0; i < arr.Count(); i++)
                {
                    var s = EnumExtension(arr.ToArray()[i]);
                    if (displayName.Substring(0,2).Equals(s))
                    {
                        intRst =i;
                        break;
                    }
                }
                return intRst;
            }
            catch (Exception)
            {
                return -1;
            }                     
        }

        public static string PageDom(string list_type, int totalCount, int pageIndex, int pageSize=10 , int centSize=20)
        {
            //计算页数
            if (totalCount < 1 || pageSize < 1)
            {
                return "";
            }
            int pageCount = totalCount / pageSize;
            if (pageCount < 1)
            {
                return "";
            }
            if (totalCount % pageSize > 0)
            {
                pageCount += 1;
            }
            if (pageCount <= 1)
            {
                return "";
            }
            StringBuilder pageStr = new StringBuilder();
            string firstBtn = $"<a href='javascript:GetPageList({pageIndex-1},{pageSize},{list_type});'>«上一页</a>";
            string lastBtn = $"<a href='javascript:GetPageList({pageIndex+1},{pageSize},{list_type});'>下一页»</a>";
            string firstStr = $"<a href='javascript:GetPageList(1,{pageSize},{list_type});'>1</a>";
            string lastStr = $"<a href='javascript:GetPageList({pageCount},{pageSize},{list_type});'>{pageCount}</a>";

            if (pageIndex <= 1)
            {
                firstBtn = "<span class=\"disabled\">«上一页</span>";
            }
            if (pageIndex >= pageCount)
            {
                lastBtn = "<span class=\"disabled\">下一页»</span>";
            }
            if (pageIndex == 1)
            {
                firstStr = "<span class=\"current\">1</span>";
            }
            if (pageIndex == pageCount)
            {
                lastStr = "<span class=\"current\">" + pageCount.ToString() + "</span>";
            }
            int firstNum = pageIndex - (centSize / 2); //中间开始的页码
            if (pageIndex < centSize)
                firstNum = 2;
            int lastNum = pageIndex + centSize - ((centSize / 2) + 1); //中间结束的页码
            if (lastNum >= pageCount)
                lastNum = pageCount - 1;
            pageStr.Append("<span>共" + totalCount + "记录</span>");
            pageStr.Append(firstBtn + firstStr);
            if (pageIndex >= centSize)
            {
                pageStr.Append("<span>...</span>\n");
            }
            for (int i = firstNum; i <= lastNum; i++)
            {
                if (i == pageIndex)
                {
                    pageStr.Append("<span class=\"current\">" + i + "</span>");
                }
                else
                {
                    //pageStr.Append("$<a href='javascript:GetPageList(" + i.ToString() + ", " + pageSize + ");'>" + i + "</a>");
                    pageStr.Append($"<a href='javascript:GetPageList({i},{pageSize},{list_type});'>{i}</a>");
                }
            }
            if (pageCount - pageIndex > centSize - ((centSize / 2)))
            {
                pageStr.Append("<span>...</span>");
            }
            pageStr.Append(lastStr + lastBtn);
            return pageStr.ToString();
        }
        
        public static int DelFiles(List<string> paths)
        {
            int i = 0;
            foreach (var item in paths)
            {
                i += DelFile(item);
            }
            return i;
        }

        public static int DelFile(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                File.Delete(path);
                return 1;
            }
            return 0;
        }

        public static Task DelFiles(string folder, List<string> listPath)
        {
            foreach (var item in listPath)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    Task.Run(() => DelFile(folder + item));
                }             
            }
            return Task.CompletedTask;
        }

        //public static string PageDom(string list_type, int totalCount, int pageIndex, string js_pageChanger, int pageSize = 10, int centSize = 20)
        //{
        //    if (js_pageChanger.Equals("GetPageList"))
        //    {
        //        return PageDom(list_type, totalCount, pageIndex, pageSize, centSize);
        //    }
        //    else
        //    {

        //    }

        //    throw new NotImplementedException();
        //}
      
    }
}
