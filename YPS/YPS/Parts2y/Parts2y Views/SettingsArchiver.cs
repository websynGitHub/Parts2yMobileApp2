using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Xamarin.Forms;
using YPS.CommonClasses;

namespace YPS.Parts2y.Parts2y_Views
{
    public interface IArchiver
    {
        void ArchiveText(string filename, string text);
        string UnarchiveText(string filename);
    }
    public class SettingsArchiver
    {
        public static void ArchiveSettings(ScanerSettings settings)
        {
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(ScanerSettings));
                using (StringWriter textWriter = new StringWriter())
                {
                    serializer.Serialize(textWriter, settings);
                    var serializedSettings = textWriter.ToString();
                    DependencyService.Get<IArchiver>().ArchiveText("settings.xml", serializedSettings);
                }

            }
            catch (Exception ex)
            {

            }
        }

        public static ScanerSettings UnarchiveSettings()
        {
            try
            {

                var serializedSettings = DependencyService.Get<IArchiver>().UnarchiveText("settings.xml");
                if (string.IsNullOrEmpty(serializedSettings))
                {
                    return new ScanerSettings();
                }
                XmlSerializer serializer = new XmlSerializer(typeof(ScanerSettings));
                using (StringReader textReader = new StringReader(serializedSettings))
                {
                    return (ScanerSettings)serializer.Deserialize(textReader);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
