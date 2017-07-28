//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Tunynet.Utilities;
using Tunynet.Repositories;
using Tunynet.Settings;
using Tunynet.FileStore;

namespace Tunynet.Common
{

    /// <summary>
    /// 表情业务逻辑类
    /// </summary>
    public class EmotionService
    {
        private string _emoticonConfigName = "Emotions.xml";
        private string _emoticonPath = "/img/Emotions";
        private string _fullName = string.Empty;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="emotionSettings"></param>
        public EmotionService()
        {
            DirectoryInfo dir = new DirectoryInfo(WebUtility.GetPhysicalFilePath(_emoticonPath));
            foreach (var info in dir.GetDirectories())
            {
                PopulateEmotionCategory(info.Name, info.FullName);
                _fullName = info.FullName;
            }

        }

        #region Get/Gets

        /// <summary>
        /// 根据CategoryID获取EmotionCategory
        /// </summary>
        /// <param name="directoryName">表情分类目录名</param>
        public List<Emotion> GetEmotionCategory(string directoryName)
        {
            List<Emotion> emotionList = new List<Emotion>();
            if (string.IsNullOrEmpty(directoryName))
                return emotionList;

            LoadEmoticons(directoryName, out emotionList);
            return emotionList;
        }


        #endregion

        #region EmoticonPackageProcess

        /// <summary>
        /// 加载表情分类下的表情
        /// </summary>
        /// <param name="emotionCategory">表情分类</param>
        public void LoadEmoticons(string directoryName, out List<Emotion> emotionList)
        {
            List<Emotion> emotions = new List<Emotion>();
            XElement document = XElement.Load(_fullName + "\\" + _emoticonConfigName);
            if (document != null)
            {
                //为启用的分类加载表情
                PopulateEmotion(directoryName, document, out emotions);
            }
            emotionList = emotions;
            return;
        }


        /// <summary>
        /// 组装表情分类实体
        /// </summary>
        /// <param name="directoryName">目录名</param>
        /// <param name="fullPath">表情包目录</param>
        private void PopulateEmotionCategory(string directoryName, string fullPath)
        {
            List<Emotion> emotions = new List<Emotion>();
            if (string.IsNullOrEmpty(directoryName))
                return;
            string configFullName = fullPath + "\\" + _emoticonConfigName;
            if (!File.Exists(configFullName))
            {
                Directory.Delete(fullPath, true);
                throw new ExceptionFacade("找不到文件：" + configFullName + ",您上传的不是表情包");
            }
            XElement document = XElement.Load(configFullName);
            if (document == null)
            {
                Directory.Delete(fullPath, true);
                return;
            }

            PopulateEmotion(directoryName, document, out emotions);
        }

        /// <summary>
        /// 组装表情实体
        /// </summary>
        /// <param name="category">表情分类</param>
        /// <param name="categoryElement">表情分类配置节点</param>
        private void PopulateEmotion(string directoryName, XElement categoryElement, out List<Emotion> emotions)
        {
            var emotionList = new List<Emotion>();
            IEnumerable<XElement> emotionsElements = categoryElement.Elements();

            foreach (var emotionElement in emotionsElements)
            {
                Emotion emotion = Emotion.New(emotionElement);
                emotion.FormatedCode = string.Format("[{0}]", emotion.Code);
                emotion.ImageUrl = string.Format("{0}/{1}/{2}", _emoticonPath, directoryName, emotion.FileName);

                if (emotionList.Where(n => n.Code == emotion.Code).Count() == 0)
                    emotionList.Add(emotion);
                PrepareTransformsInfo(emotionList);
            }
            emotions = emotionList;
        }

        #endregion

        #region Emoticon & User Transforms

        private Dictionary<string, string> emoctionDictionary = new Dictionary<string, string>();
        private Char firstCharCheck = '[';
        private BitArray allCharCheck = new BitArray(char.MaxValue);
        private int maxWordLength = 0;
        private int minWordLength = int.MaxValue;

        /// <summary>
        /// 将所有表情写入集合中供下面的转换
        /// </summary>
        /// <param name="category">表情分类</param>
        private void PrepareTransformsInfo(List<Emotion> emotions)
        {
            if (emotions == null)
                return;

            foreach (Emotion emotion in emotions)
            {
                string smileyPattern = emotion.FormatedCode;
                string replacePattern = string.Format("<img src=\"{0}\" title=\"{1}\" alt=\"{1}\" />", emotion.ImageUrl, emotion.Description);

                maxWordLength = Math.Max(maxWordLength, smileyPattern.Length);
                minWordLength = Math.Min(minWordLength, smileyPattern.Length);

                foreach (char c in smileyPattern)
                {
                    allCharCheck[c] = true;
                }
                emoctionDictionary[smileyPattern] = replacePattern;
            }
        }

        /// <summary>
        /// 以含表情图片的html代码替换字符串formattedPost中的SmileyCode
        /// </summary>
        /// <param name="formattedPost">被替换的字符串</param>
        public string EmoticonTransforms(string formattedPost)
        {
            List<string> needReplacedSub = new List<string>();
            int index = 0;
            while (index < formattedPost.Length)
            {
                if (formattedPost[index] != firstCharCheck)
                {
                    index++;
                    continue;
                }

                for (int j = 1; j <= Math.Min(maxWordLength, formattedPost.Length - index); j++)
                {
                    if (allCharCheck[formattedPost[index + j - 1]] == false)
                    {
                        continue;
                    }

                    string sub = formattedPost.Substring(index, j);

                    if (emoctionDictionary.ContainsKey(sub))
                    {
                        needReplacedSub.Add(sub);
                    }
                }

                index++;
            }

            for (int i = 0; i < needReplacedSub.Count; i++)
            {
                formattedPost = formattedPost.Replace(needReplacedSub[i], emoctionDictionary[needReplacedSub[i]]);
            }

            return formattedPost;
        }

        #endregion
    }
}