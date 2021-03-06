﻿using System;
using HelloWorldApp.Common;
using System.Runtime.Serialization;
using System.IO;

namespace HelloWorldApp.DataModel
{
    /// <summary>
    /// ハローワールドアプリケーションのコアロジック
    /// </summary>
    public class HelloWorldModel : BindableBase
    {
        // デフォルトのインスタンス
        private static HelloWorldModel defaultInstance;

        /// <summary>
        /// デフォルトのインスタンスを取得します。
        /// </summary>
        /// <returns>デフォルトのインスタンス</returns>
        public static HelloWorldModel GetDefault()
        {
            if (defaultInstance == null)
            {
                defaultInstance = new HelloWorldModel();
            }

            return defaultInstance;
        }

        /// <summary>
        /// デフォルトのインスタンスをStreamへ保存します。
        /// </summary>
        /// <param name="s">保存用のストリーム</param>
        public static void SaveToStream(Stream s)
        {
            var serializer = new DataContractSerializer(typeof(HelloWorldModel));
            serializer.WriteObject(s, GetDefault());
        }

        /// <summary>
        /// デフォルトのインスタンスをStreamから読み込みます。
        /// </summary>
        /// <param name="s">読み込み用のストリーム</param>
        public static void LoadFromStream(Stream s)
        {
            var serializer = new DataContractSerializer(typeof(HelloWorldModel));
            defaultInstance = serializer.ReadObject(s) as HelloWorldModel;
        }
        
        private string name;
        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

        private string message;
        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        [IgnoreDataMember]
        public string Message
        {
            get { return this.message; }
            set { this.SetProperty(ref this.message, value); }
        }

        // 初期値は朝
        private string time = "朝";
        /// <summary>
        /// 時間帯を取得または設定します。有効な値は朝・昼・晩のいずれかです。
        /// </summary>
        [IgnoreDataMember]
        public string Time
        {
            get { return this.time; }
            set { this.SetProperty(ref this.time, value); }
        }

        /// <summary>
        /// 名前と時間帯から適切なメッセージを生成します。
        /// </summary>
        public void Greet()
        {
            // 出力メッセージのフォーマットを格納するための変数
            string format = null;

            // 選択項目に応じて出力メッセージのフォーマットを設定する
            switch (this.Time)
            {
                case "朝":
                    format = "おはようございます。{0}さん。";
                    break;
                case "昼":
                    format = "こんにちは。{0}さん。";
                    break;
                case "晩":
                    format = "こんばんは。{0}さん。";
                    break;
                default:
                    // 朝と昼と晩しかありえない
                    throw new InvalidOperationException("不正な値");
            }

            // 出力メッセージをテキストブロックに設定する
            this.Message = string.Format(format, this.Name);

        }
    }
}
