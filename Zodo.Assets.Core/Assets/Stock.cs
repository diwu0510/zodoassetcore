using HZC.Database;
using HZC.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_Stock")]
    public class Stock : Entity
    {
        /// <summary>
        /// �̵����
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// �̵��Ƿ����
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public bool IsFinish { get; set; } = false;

        /// <summary>
        /// �̵����ʱ��
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? FinishAt { get; set; }

        [MyDataField(Ignore = true)]
        public List<StockItem> Items { get; set; }
    }
}
