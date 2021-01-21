//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Reflection;
//using System.Text;
//using SQLite;
//using SQLiteNetExtensions.Extensions;
//using Xamarin.Forms;
//using YPSePOD.ypsepod.CommonClasses;
//using YPSePOD.ypsepod.Model;
//using static YPSePOD.ypsepod.Model.PhotoModel;
//using static YPSePOD.ypsepod.Model.SqliteModels.LocalVehicleDetailsModel;

//namespace YPSePOD.ypsepod.SQLITE
//{
//    public class VehicleDetailsDB
//    {
//        private SQLiteConnection _conn;

//        public VehicleDetailsDB(string table)
//        {
//            _conn = DependencyService.Get<ISqlite>().GetConnection();

//            if (table == "vedetails")
//            {
//                _conn.CreateTable<VehicleDetailsModel>();
//            }
//            else if (table == "image")
//            {
//                _conn.CreateTable<CustomPhotoModel>();
//            }
//            else if (table == "loaddata")
//            {
//                _conn.CreateTable<GetLoaddata>();
//            }
//        }


//        public void SaveAndUpdateAllVeData(List<VehicleDetailsModel> veAllData)
//        {
//            try
//            {
//                var val = GetAllVeData();

//                if (_conn == null)
//                {
//                    _conn = DependencyService.Get<ISqlite>().GetConnection();
//                    _conn.CreateTable<VehicleDetailsModel>();
//                }

//                if (val.Count > 0)
//                {
//                    _conn.UpdateWithChildren(val[0]);
//                }
//                else
//                {
//                    veAllData[0].vinno = Settings.VINNo;
//                    _conn.InsertWithChildren(veAllData[0], false);
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public List<VehicleDetailsModel> GetAllVeData()
//        {
//            try
//            {
//                var val = _conn.GetAllWithChildren<VehicleDetailsModel>(c => c.vinno == Settings.VINNo);
//                return val;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public void UpdateAllVeData(List<VehicleDetailsModel> data)
//        {
//            try
//            {
//                _conn.UpdateWithChildren(data[0]);
//            }
//            catch (Exception ex)
//            {

//            }
//        }


//        public void SaveImage(ObservableCollection<CustomPhotoModel> photo)
//        {
//            try
//            {
//                _conn.InsertAll(photo);
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        //public void SaveAndUpdateSignature(CPQuestionsdata signature)
//        //{
//        //    try
//        //    {
//        //        signature.cpquestions = signature;

//        //        _conn.CreateTable<VehicleDetailsModel>();
//        //        //if (isSave == true)
//        //        //{
//        //        _conn.Insert(signature);
//        //        //}
//        //        //else
//        //        //{
//        //        //    _conn.Update(signature);
//        //        //}
//        //    }
//        //    catch (Exception ex)
//        //    {

//        //    }
//        //}

//        //public SignatureImagesModel GetSignature(int type)
//        //{
//        //    try
//        //    {
//        //        _conn.CreateTable<VehicleDetailsModel>();
//        //        var val = _conn.Table<VehicleDetailsModel>().Where(wr => wr.cpquestions. == Settings.VINNo).FirstOrDefault();
//        //        return val;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return null;
//        //    }
//        //}


//        public void SaveUpdateLoadData(List<GetLoaddata> loadData)
//        {
//            try
//            {
//                var loaddata = GetLoadData();

//                if (loaddata.Count > 0)
//                {
//                    _conn.UpdateWithChildren(loaddata[0]);
//                }
//                else
//                {
//                    loadData[0].loadno = Settings.LoadNo;
//                    _conn.CreateTable<GetLoaddata>();
//                    _conn.InsertAllWithChildren(loadData, false);
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public List<GetLoaddata> GetLoadData()
//        {
//            try
//            {
//                var data = _conn.GetAllWithChildren<GetLoaddata>(wr => wr.loadno == Settings.LoadNo);
//                return data;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public List<CustomPhotoModel> GetImage()
//        {
//            try
//            {
//                var data = _conn.Table<CustomPhotoModel>().Where(wr => wr.vinno == Settings.VINNo && wr.queseq == Settings.QNo_Sequence).ToList();
//                return data;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        //public void DeletetAllImage()
//        //{
//        //    try
//        //    {
//        //        var data = _conn.DeleteAll<CustomPhotoModel>();
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //    }
//        //}


//        public void SaveQuestionsList(object questionslist)
//        {
//            try
//            {

//                var val = questionslist as CPQuestionssList2;

//                var type = questionslist.GetType();

//                PropertyInfo prop = type.GetProperty("listOptions");

//                Settings.VINNo = Convert.ToString(prop.GetValue(questionslist, null));

//                _conn = DependencyService.Get<ISqlite>().GetConnection();
//                _conn.CreateTable<CPQuestions>();
//                //_conn = DependencyService.Get<ISqlite>().GetConnection();
//                _conn.CreateTable<ListOfOption>();
//                _conn.InsertAll(val.CPQuestionsdata as IEnumerable<ListOfOption>);
//                _conn.InsertAll(val.CPQuestionsdata as IEnumerable<CPQuestions>);
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public void SaveOptionsforQues(object optlist)
//        {
//            try
//            {
//                var val = optlist as AllVehicleDetails;
//                _conn = DependencyService.Get<ISqlite>().GetConnection();
//                _conn.CreateTable<CPQuestions2>();
//                _conn.CreateTable<ListOfOption>();
//                _conn.InsertAll(val.cpquestions2 as IEnumerable<CPQuestions2>);

//                foreach (var list in val.cpquestions2)
//                {
//                    _conn.InsertAll(list.listOptions as IEnumerable<ListOfOption>);
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }
//        public List<CPQuestions2> GetAllGuestions()
//        {
//            try
//            {
//                _conn = DependencyService.Get<ISqlite>().GetConnection();
//                _conn.CreateTable<CPQuestions2>();
//                var data = _conn.Table<CPQuestions2>().Where(wr => wr.vinno == Settings.VINNo).ToList();
//                return data;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public void ClearAllData()
//        {
//            try
//            {
//                _conn.DeleteAll<CPQuestions2>();
//                _conn.DeleteAll<ListOfOption>();
//            }
//            catch (Exception ex)
//            {
//            }
//        }

//        public List<ListOfOption> GetListOfOptions()
//        {
//            try
//            {
//                _conn = DependencyService.Get<ISqlite>().GetConnection();
//                _conn.CreateTable<ListOfOption>();
//                var data = _conn.Table<ListOfOption>().Where(wr => wr.vinno == Settings.VINNo).ToList();
//                return data;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }
//    }
//}
