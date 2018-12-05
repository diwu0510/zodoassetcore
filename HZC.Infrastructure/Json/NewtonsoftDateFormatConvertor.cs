using Newtonsoft.Json.Converters;

namespace HZC.Infrastructure
{
    /// <summary>
    /// JSON.NET�����ڸ�ʽת��
    /// �÷���[JsonConverter(typeof(DateFormatConverter))]
    /// </summary>
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }

    /// <summary>
    /// JSON.NET������ʱ���ʽת��
    /// /// �÷���[JsonConverter(typeof(DateTimeFormatConverter))]
    /// </summary>
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm";
        }
    }
}
