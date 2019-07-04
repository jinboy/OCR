using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Media;
using YunZhi;

namespace YunZhi
{
    public partial class YunZhiForm : Form
    {
      
        public YunZhiForm()
        {
            InitializeComponent();
        }

        static string[] strMemberType = new string[]{
            "临时车",
            "月卡",
            "贵宾卡",
            "储值卡",
            "领导车",
            "特殊车",
            "内部车",
            "白名单",
            "黑名单"
        };

        static string [] szPlateDefaultWord = new string[]{
	        "京",
	        "津",
	        "沪",
	        "渝",
	        "冀",
	        "晋",
	        "辽",
	        "吉",
	        "黑",
	        "苏",
	        "浙",
	        "皖",
	        "闽",
	        "赣",
	        "鲁",
	        "豫",
	        "鄂",
	        "湘",
	        "粤",
	        "琼",
	        "川",
	        "贵",
	        "云",
	        "陕",
	        "甘",
	        "宁",
	        "青",
	        "藏",
	        "桂",
	        "蒙",
	        "新",
	        "全国"
        };

        //去掉I，0两个英文字母
        static byte [] g_ucLocalCity = new byte[26 - 2 + 1];
        static uint [] g_uiPlateDefaultWord  = new uint [32];

        private static string strImageDir = "";
        private static int nCamId = -1;
        //private static MyClass.FGetImageCB fGetImageCB;
        private static MyClass.FGetImageCB2 fGetImageCB2;

        private static MyClass.FGetOffLinePayRecordCB fGetOffLinePayRecordCB;
        private static MyClass.FGetOffLineImageCBEx fGetOffLineImageCBEx;
        private static MyClass.FNetFindDeviceCallback fNetFindDeviceCallback;
        private static MyClass.FGetReportCBEx fGetReportCBEx;
        private static MyClass.FTalkConnStateCallBack fGTalkConnStateCallBack;

        private static MyClass.T_VideoDetectParamSetup tVideoDetectParamSetup;
        private static MyClass.T_RS485Data tRS485Data;
        private static MyClass.T_DCTimeSetup tDCTimeSetup;
        private static MyClass.T_VehicleVAFunSetup tVehicleVAFunSetup;

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="ucLocalCity"></param>
        /// <returns></returns>
        static int GetLocalCityIndex(byte ucLocalCity)
        {
            int nIndex = 0;
            for (int i = 0; i < g_ucLocalCity.Length; i++)
            {
                if (ucLocalCity == g_ucLocalCity[i])
                {
                    nIndex = i;
                    break;
                }
            }

            return nIndex;
        }

        public static uint Reverse_uint(uint uiNum)
        {
            return ((uiNum & 0x000000FF) << 24) |
                   ((uiNum & 0x0000FF00) << 8) |
                   ((uiNum & 0x00FF0000) >> 8) |
                   ((uiNum & 0xFF000000) >> 24);
        }

        /// <summary>
        /// 车牌识别结果上报回调
        /// </summary>
        /// <param name="tHandle">相机句柄[0-31],该句柄是由Net_AddCamera反回的相机管理句柄</param>
        /// <param name="uiImageId"></param>
        /// <param name="tImageInfo"></param>
        /// <param name="tPicInfo"></param>
        /// <returns></returns>
        private int FGetImageCB(int tHandle, uint uiImageId, ref MyClass.T_ImageUserInfo tImageInfo, ref MyClass.T_PicInfo tPicInfo)
        {
            if (tHandle == nCamId)
            {
                string strTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmssfff");// 年月日时分秒
                // 车辆图像
                if (tImageInfo.ucViolateCode == 0)
                {
                    string szLprResult = System.Text.Encoding.Default.GetString(tImageInfo.szLprResult).Replace("\0", "");
                    labelPlateNum.Text = szLprResult;

                    switch (tImageInfo.ucVehicleSize)//车型
                    {
                        case 0:
                            {
                                labelVehicSize.Text = "未知车型";
                                break;
                            }

                        case 1:
                            {
                                labelVehicSize.Text = "大型车";
                                break;
                            }
                        case 2:
                            {
                                labelVehicSize.Text = "中型车";
                                break;
                            }
                        case 3:
                            {
                                labelVehicSize.Text = "小型车";
                                break;
                            }
                        case 4:
                            {
                                labelVehicSize.Text = "摩托车";
                                break;
                            }
                        case 5:
                            {
                                labelVehicSize.Text = "行人";
                                break;
                            }
                        default:
                            {
                                labelVehicSize.Text = "未知车型";
                                break;
                            }
                    }

                    switch (tImageInfo.ucPlateColor)//车牌颜色
                    {
                        case 0:
                            labelPlateColor.Text = "蓝色";
                            labelPlateNum.BackColor = Color.Blue;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 1:
                            labelPlateColor.Text = "黄色";
                            labelPlateNum.BackColor = Color.Yellow;
                            labelPlateNum.ForeColor = Color.Black;
                            break;
                        case 2:
                            labelPlateColor.Text = "白色";
                            labelPlateNum.BackColor = Color.White;
                            labelPlateNum.ForeColor = Color.Black;
                            break;
                        case 3:
                            labelPlateColor.Text = "黑色";
                            labelPlateNum.BackColor = Color.Black;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 5:
                            labelPlateColor.Text = "绿色";
                            labelPlateNum.BackColor = Color.Green;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 6:
                            labelPlateColor.Text = "黄绿";
                            labelPlateNum.BackColor = Color.Green;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 4:
                        default:
                            labelPlateNum.Text = "未识别";
                            labelPlateNum.BackColor = Color.Blue;
                            break;
                    }

                    if (tPicInfo.ptPanoramaPicBuff != IntPtr.Zero && tPicInfo.uiPanoramaPicLen != 0)
                    {
                        byte[] BytePanoramaPicBuff = new byte[tPicInfo.uiPanoramaPicLen];
                        Marshal.Copy(tPicInfo.ptPanoramaPicBuff, BytePanoramaPicBuff, 0, (int)tPicInfo.uiPanoramaPicLen);
                        string strImageFile = String.Format("{0}\\{1}.jpg", strImageDir, strTime);
                        FileStream fs = new FileStream(strImageFile, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                        fs.Write(BytePanoramaPicBuff, 0, (int)tPicInfo.uiPanoramaPicLen);
                        pictureBoxPlate.Image = Image.FromStream(fs);
                        fs.Close();
                        fs.Dispose();
                    }

                    if (tPicInfo.ptVehiclePicBuff != IntPtr.Zero && tPicInfo.uiVehiclePicLen != 0)
                    {
                        byte[] ByteVehiclePicBuff = new byte[tPicInfo.uiVehiclePicLen];
                        Marshal.Copy(tPicInfo.ptVehiclePicBuff, ByteVehiclePicBuff, 0, (int)tPicInfo.uiVehiclePicLen);
                        string strImageFile = String.Format("{0}\\{1}_plate.jpg", strImageDir, strTime);
                        FileStream fs = new FileStream(strImageFile, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                        fs.Write(ByteVehiclePicBuff, 0, (int)tPicInfo.uiVehiclePicLen);
                        pictureBoxPlateImage.Image = Image.FromStream(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 车牌识别结果上报回调
        /// </summary>
        /// <param name="tHandle">相机句柄[0-31],该句柄是由Net_AddCamera反回的相机管理句柄</param>
        /// <param name="uiImageId">图片ID</param>
        /// <param name="tImageInfo">识别结果结构体，具体信心见 T_ImageUserInfo</param>
        /// <param name="tPicInfo">图片缓冲区结构体</param>
        /// <returns></returns>
        private int FGetImageCB2(int tHandle, uint uiImageId, ref MyClass.T_ImageUserInfo2 tImageInfo, ref MyClass.T_PicInfo tPicInfo)
        {
            if (tHandle == nCamId)
            {
                string strTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmssfff");
                //车辆图像
                if (tImageInfo.ucViolateCode == 0)
                {
                    string szLprResult = System.Text.Encoding.Default.GetString(tImageInfo.szLprResult).Replace("\0", "");
                    labelPlateNum.Text = szLprResult;

                    //车型
                    string strVehicleBrand = System.Text.Encoding.Default.GetString(tImageInfo.strVehicleBrand).Replace("\0", "");
                    labeVehicleBrand.Text = strVehicleBrand;

                    switch (tImageInfo.ucVehicleSize)//车辆类型
                    {
                        case 0:
                            {
                                labelVehicSize.Text = "未知车型";
                                break;
                            }

                        case 1:
                            {
                                labelVehicSize.Text = "大型车";
                                break;
                            }
                        case 2:
                            {
                                labelVehicSize.Text = "中型车";
                                break;
                            }
                        case 3:
                            {
                                labelVehicSize.Text = "小型车";
                                break;
                            }
                        case 4:
                            {
                                labelVehicSize.Text = "摩托车";
                                break;
                            }
                        case 5:
                            {
                                labelVehicSize.Text = "行人";
                                break;
                            }
                        default:
                            {
                                labelVehicSize.Text = "未知车型";
                                break;
                            }
                    }

                    switch (tImageInfo.ucPlateColor)//车牌颜色
                    {
                        case 0:
                            labelPlateColor.Text = "蓝色";
                            labelPlateNum.BackColor = Color.Blue;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 1:
                            labelPlateColor.Text = "黄色";
                            labelPlateNum.BackColor = Color.Yellow;
                            labelPlateNum.ForeColor = Color.Black;
                            break;
                        case 2:
                            labelPlateColor.Text = "白色";
                            labelPlateNum.BackColor = Color.White;
                            labelPlateNum.ForeColor = Color.Black;
                            break;
                        case 3:
                            labelPlateColor.Text = "黑色";
                            labelPlateNum.BackColor = Color.Black;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 5:
                            labelPlateColor.Text = "绿色";
                            labelPlateNum.BackColor = Color.Green;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 6:
                            labelPlateColor.Text = "黄绿";
                            labelPlateNum.BackColor = Color.Green;
                            labelPlateNum.ForeColor = Color.White;
                            break;
                        case 4:
                        default:
                            labelPlateNum.Text = "未识别";
                            labelPlateNum.BackColor = Color.Blue;
                            break;
                    }

                    
                    string plate = System.Text.Encoding.Default.GetString(tImageInfo.szLprResult).Replace("\0", "");// 解析车牌
                    /* ***********数据初始化 start************** */
                    PlateBO plateBO = new PlateBO();
                    plateBO.plate = plate;
                    WashRecordBO washRecordBO = new WashRecordBO();
                    washRecordBO.id = Utils.sid();
                    washRecordBO.plate = plate;
                    
                    SoundPlayer soundWelcome = new SoundPlayer("welcome.wav");// 欢迎音频
                    SoundPlayer soundNextWeek = new SoundPlayer("nextweek.wav");// 
                    SoundPlayer soundLeave = new SoundPlayer("leave.wav");

                    var customDay = textBoxDayOfCarWashLimit.Text;
                    /* ***********数据初始化 end************** */

                    /* *********** 规定时间洗车相关 start************** */
                    // 1.先查询，若查询不到，该车首次来，直接洗车
                    // 2. 若该车第二次来，或者多次来
                    // 3. 若该车规定时间内洗过（查询出时间比对），弹框，该车规定时间之内洗过
                    // 4. 若该车规定时间之内未洗过，将该车信息存入数据库，同时生成该车新的洗车时间
                    MysqlUtils mysqlUtils = new MysqlUtils();
                    List<string>[] plateList = mysqlUtils.PriciseSelect(plateBO);// 精确查询车牌库
                    List<string> carWashRecordList = mysqlUtils.PriciseSelectCarWashRecord(washRecordBO);// 精确查询洗车记录库
                    List<string> carWashRecordTimeList = new List<string>();
                    foreach (string item in carWashRecordList)
                    {
                        string[] str = item.Split(',');
                        string carWashRecordTime = str[2];// 洗车日期
                        carWashRecordTimeList.Add(carWashRecordTime);
                    }
                    Console.WriteLine(carWashRecordList);
                    if (plateList[0].Count == 0)// 车牌库没有该车辆
                    {// 未查询到该车，首次来
                        //MessageBox.Show("该车辆首次来访");
                        soundLeave.Play();// 首次来访车辆定为临时车
                        //mysqlUtils.Insert(plateBO);// 允许洗车之后车辆数据入库
                        //mysqlUtils.InsertIntoWashRecord(washRecordBO);
                        MessageBox.Show("临时车，若允许，请在软件左下角将该车辆信息导入新增至白名单！");
                    }
                    else if (plateList[0].Count > 0)// 车牌库有该车辆
                    {
                        // 库内车辆未洗过
                        if (carWashRecordTimeList.Count == 0)
                        {
                            soundWelcome.Play();
                            MessageBox.Show("允许洗车");

                            //mysqlUtils.Insert(plateBO);// 允许洗车之后车辆数据入库
                            mysqlUtils.InsertIntoWashRecord(washRecordBO);// 新的洗车记录增加至洗车记录表

                            // 抬闸
                            MyClass.T_ControlGate tControlGate = new MyClass.T_ControlGate();
                            tControlGate.ucState = 1;
                            tControlGate.ucReserved = new byte[3] { 0, 0, 0 };

                            int iRet = MyClass.Net_GateSetup(nCamId, ref tControlGate);
                            if (iRet == 0)
                            {
                                MessageBox.Show("抬闸成功", "提示");
                            }
                            else
                            {
                                MessageBox.Show("抬闸失败", "提示");
                            }
                        }
                        else
                        {
                            var lastWashTime = carWashRecordTimeList[carWashRecordTimeList.Count - 1];// 上次洗车时间
                            if (Utils.isLessThanCustomDays(lastWashTime,
                                DateTime.Now.ToString(),
                                Convert.ToInt32(customDay)))
                            {// 不足规定时间
                                soundNextWeek.Play();
                                MessageBox.Show("该车辆规定时间之内洗过");
                            }
                            else
                            {// 超过规定时间，允许洗车
                                soundWelcome.Play();
                                MessageBox.Show("允许洗车");

                                //mysqlUtils.Insert(plateBO);// 允许洗车之后车辆数据入库
                                mysqlUtils.InsertIntoWashRecord(washRecordBO);// 新的洗车记录增加至洗车记录表

                                // 抬闸
                                MyClass.T_ControlGate tControlGate = new MyClass.T_ControlGate();
                                tControlGate.ucState = 1;
                                tControlGate.ucReserved = new byte[3] { 0, 0, 0 };

                                int iRet = MyClass.Net_GateSetup(nCamId, ref tControlGate);
                                if (iRet == 0)
                                {
                                    MessageBox.Show("抬闸成功", "提示");
                                }
                                else
                                {
                                    MessageBox.Show("抬闸失败", "提示");
                                }
                            }
                        }
                        
                        /* *********** 规定时间洗车相关 end************** */
                    }
                    
                    // 识别图像文件存储
                    if (tPicInfo.ptPanoramaPicBuff != IntPtr.Zero && tPicInfo.uiPanoramaPicLen != 0)
                    {
                        byte[] BytePanoramaPicBuff = new byte[tPicInfo.uiPanoramaPicLen];
                        Marshal.Copy(tPicInfo.ptPanoramaPicBuff, BytePanoramaPicBuff, 0, (int)tPicInfo.uiPanoramaPicLen);
                        string strImageFile = String.Format("{0}\\{1}_车牌.jpg", strImageDir, strTime);
                        FileStream fs = new FileStream(strImageFile, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                        fs.Write(BytePanoramaPicBuff, 0, (int)tPicInfo.uiPanoramaPicLen);
                        pictureBoxPlate.Image = Image.FromStream(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                    // 识别车牌图像文件存储
                    if (tPicInfo.ptVehiclePicBuff != IntPtr.Zero && tPicInfo.uiVehiclePicLen != 0)
                    {
                        byte[] ByteVehiclePicBuff = new byte[tPicInfo.uiVehiclePicLen];
                        Marshal.Copy(tPicInfo.ptVehiclePicBuff, ByteVehiclePicBuff, 0, (int)tPicInfo.uiVehiclePicLen);
                        string strImageFile = String.Format("{0}\\{1}_plate.jpg", strImageDir, strTime);
                        FileStream fs = new FileStream(strImageFile, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                        fs.Write(ByteVehiclePicBuff, 0, (int)tPicInfo.uiVehiclePicLen);
                        pictureBoxPlateImage.Image = Image.FromStream(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 脱机收费数据回调
        /// </summary>
        /// <param name="tHandle"></param>
        /// <param name="ucType"></param>
        /// <param name="ptVehPayInfo"></param>
        /// <param name="uiLen"></param>
        /// <param name="ptPicInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int FGetOffLinePayRecordCB(int tHandle, byte ucType, ref MyClass.T_VehPayRsp ptVehPayInfo, uint uiLen, ref MyClass.T_PicInfo ptPicInfo, IntPtr obj)
        {
            if (tHandle == nCamId)
            {
                ListViewItem lvi = new ListViewItem(System.Text.Encoding.Default.GetString(ptVehPayInfo.acPlate), 0);
                lvi.SubItems.Add(System.Text.Encoding.Default.GetString(ptVehPayInfo.acEntryTime));
                lvi.SubItems.Add(System.Text.Encoding.Default.GetString(ptVehPayInfo.acExitTime));
                lvi.SubItems.Add(String.Format("{0}元", ptVehPayInfo.uiRequired / 10));
                lvi.SubItems.Add(String.Format("{0}元", ptVehPayInfo.uiPrepaid / 10));
                lvi.SubItems.Add(ptVehPayInfo.ucVehType == 1 ? "小车" : "大车");
                lvi.SubItems.Add(strMemberType[ptVehPayInfo.ucUserType]);
                lvi.SubItems.Add(ptVehPayInfo.ucResultCode == 1 ? "成功" : "无记录");
                //listViewOffLinePayRecord.Items.Add(lvi);
            }
           
            return 0;
        }

        /// <summary>
        /// 脱机数据回调
        /// </summary>
        /// <param name="tHandle"></param>
        /// <param name="uiImageId"></param>
        /// <param name="tImageInfo"></param>
        /// <param name="tPicInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int FGetOffLineImageCBEx(int tHandle, uint uiImageId, ref MyClass.T_ImageUserInfo tImageInfo, ref MyClass.T_PicInfo tPicInfo, IntPtr obj)
        {
            if (tHandle == nCamId)
            {
                //string strTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmssfff");
                ////车辆图像
                //if (tImageInfo.ucViolateCode == 0)
                //{
                //    string szSnapTime = System.Text.Encoding.Default.GetString(tImageInfo.acSnapTime);
                //    string szLprResult = System.Text.Encoding.Default.GetString(tImageInfo.szLprResult).Replace("\0", "");
                //    labelPlateNum.Text = szLprResult;

                //    switch (tImageInfo.ucVehicleSize)//车型
                //    {
                //        case 0:
                //            {
                //                labelVehicSize.Text = "未知车型";
                //                break;
                //            }

                //        case 1:
                //            {
                //                labelVehicSize.Text = "大型车";
                //                break;
                //            }
                //        case 2:
                //            {
                //                labelVehicSize.Text = "中型车";
                //                break;
                //            }
                //        case 3:
                //            {
                //                labelVehicSize.Text = "小型车";
                //                break;
                //            }
                //        case 4:
                //            {
                //                labelVehicSize.Text = "摩托车";
                //                break;
                //            }
                //        case 5:
                //            {
                //                labelVehicSize.Text = "行人";
                //                break;
                //            }
                //        default:
                //            {
                //                labelVehicSize.Text = "未知车型";
                //                break;
                //            }
                //    }

                //    switch (tImageInfo.ucPlateColor)//车牌颜色
                //    {
                //        case 0:
                //            labelPlateColor.Text = "蓝色";
                //            labelPlateNum.BackColor = Color.Blue;
                //            labelPlateNum.ForeColor = Color.White;
                //            break;
                //        case 1:
                //            labelPlateColor.Text = "黄色";
                //            labelPlateNum.BackColor = Color.Yellow;
                //            labelPlateNum.ForeColor = Color.Black;
                //            break;
                //        case 2:
                //            labelPlateColor.Text = "白色";
                //            labelPlateNum.BackColor = Color.White;
                //            labelPlateNum.ForeColor = Color.Black;
                //            break;
                //        case 3:
                //            labelPlateColor.Text = "黑色";
                //            labelPlateNum.BackColor = Color.Black;
                //            labelPlateNum.ForeColor = Color.White;
                //            break;
                //        case 4:
                //        default:
                //            labelPlateNum.Text = "未识别";
                //            labelPlateNum.BackColor = Color.Blue;
                //            break;
                //    }

                //    if (tPicInfo.ptPanoramaPicBuff != IntPtr.Zero && tPicInfo.uiPanoramaPicLen != 0)
                //    {
                //        byte[] BytePanoramaPicBuff = new byte[tPicInfo.uiPanoramaPicLen];
                //        Marshal.Copy(tPicInfo.ptPanoramaPicBuff, BytePanoramaPicBuff, 0, (int)tPicInfo.uiPanoramaPicLen);
                //        string strImageFile = String.Format("{0}\\{1}.jpg", strImageDir, strTime);
                //        FileStream fs = new FileStream(strImageFile, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                //        fs.Write(BytePanoramaPicBuff, 0, (int)tPicInfo.uiPanoramaPicLen);
                //        pictureBoxPlate.Image = Image.FromStream(fs);
                //        fs.Close();
                //        fs.Dispose();
                //    }

                //    if (tPicInfo.ptVehiclePicBuff != IntPtr.Zero && tPicInfo.uiVehiclePicLen != 0)
                //    {
                //        byte[] ByteVehiclePicBuff = new byte[tPicInfo.uiVehiclePicLen];
                //        Marshal.Copy(tPicInfo.ptVehiclePicBuff, ByteVehiclePicBuff, 0, (int)tPicInfo.uiVehiclePicLen);
                //        string strImageFile = String.Format("{0}\\{1}_plate.jpg", strImageDir, strTime);
                //        FileStream fs = new FileStream(strImageFile, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                //        fs.Write(ByteVehiclePicBuff, 0, (int)tPicInfo.uiVehiclePicLen);
                //        pictureBoxPlateImage.Image = Image.FromStream(fs);
                //        fs.Close();
                //        fs.Dispose();
                //    }
                //}
            }
           
            return 0;
        }

        /// <summary>
        /// 对讲链接和异常断开消息上报
        /// </summary>
        /// <param name="tHandle"></param>
        /// <param name="ucCtrlConnState"></param>
        /// <param name="obj"></param>
        private void FTalkConnStateCallBack(int tHandle, byte ucCtrlConnState, IntPtr obj)
        {
            if (ucCtrlConnState == 2)
            {
                MessageBox.Show("异常通话断开了");
            } 
        }

        private int FNetFindDeviceCallback(ref MyClass.T_RcvMsg ptFindDevice, IntPtr obj)
        {
            comboBoxIP.Items.Add(ModifyIP.IntToIp(Reverse_uint(ptFindDevice.tNetSetup.uiIPAddress)));
            return 0;
        }
        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        /*
        private int FGetReportCBEx(int tHandle, byte ucType, IntPtr ptMessage, IntPtr pUserData)
        {
            string strMsg = String.Format("{0}", ucType);

            //MessageBox.Show(strMsg);

            if (14 == ucType) //语音对讲通知
            {
                if (DialogResult.Yes == MessageBox.Show("是否接受相机端发起的语音对讲", "提示", MessageBoxButtons.YesNo))
                {
                    buttonTalk_Click(null, null);
                }
                else
                {
                    MyClass.T_AudioTalkBack tAudioTalkBack = new MyClass.T_AudioTalkBack();
                    tAudioTalkBack.enable = 4;
                    tAudioTalkBack.Reserved = new byte[5];
                    tAudioTalkBack.pressstatus = 0;
                    tAudioTalkBack.ucOutTime = 0;
                    int iRet = MyClass.Net_AudioTalkBack(tHandle, ref tAudioTalkBack);
                    if (iRet != 0)
                        MessageBox.Show("语音对讲拒绝失败，错误码：" + iRet.ToString());
                }
            }
            else if (ucType == 16)
            {
                MyClass.T_AudioLinkInfo tAudioTalkBack = new MyClass.T_AudioLinkInfo();
                byte[] pByte = new byte[Marshal.SizeOf(tAudioTalkBack)];
                Marshal.Copy(ptMessage, pByte, 0, Marshal.SizeOf(tAudioTalkBack));
                tAudioTalkBack = (MyClass.T_AudioLinkInfo)BytesToStruct(pByte, tAudioTalkBack.GetType());
                if (tAudioTalkBack.ucStatus == 1)
                    MessageBox.Show("语音对讲被被其他接听");
                    

            }

            return 0;
        }
         * */

        private void Demo_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            MyClass.Net_Init();

            if (fGTalkConnStateCallBack == null)
                fGTalkConnStateCallBack = new MyClass.FTalkConnStateCallBack(FTalkConnStateCallBack);
            MyClass.Net_RegTalkConnStateCallBack(fGTalkConnStateCallBack, IntPtr.Zero);


            int ptLen = 255;
            StringBuilder strVersion = new StringBuilder(ptLen);
            MyClass.Net_GetSdkVersion(strVersion, ref ptLen);
            // this.Text += "(" + strVersion + ")";// 这边会显示版本

            fNetFindDeviceCallback = new MyClass.FNetFindDeviceCallback(FNetFindDeviceCallback);

            strImageDir = System.Windows.Forms.Application.StartupPath + "\\image";
            if (!Directory.Exists(strImageDir))
            {
                Directory.CreateDirectory(strImageDir);
            }

            //dateTimePickerCAM.Format = DateTimePickerFormat.Custom;
            //dateTimePickerCAM.CustomFormat = "yyyy-MM-dd HH:mm:ss";

            for (int i = 0; i < szPlateDefaultWord.Length; i++ )
            {
                comboBoxProvince.Items.Add(szPlateDefaultWord[i]);
                if (i != szPlateDefaultWord.Length - 1)
                {
                     byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(szPlateDefaultWord[i]);
                     g_uiPlateDefaultWord[i] = (uint)((utf8[2] << 16) | (utf8[1] << 8) | utf8[0]);
                }
                else
                {
                    g_uiPlateDefaultWord[i] = 0;
                }
            }
            comboBoxProvince.Text = szPlateDefaultWord[szPlateDefaultWord.Length-1];

            int nLocalCityIndex = 0;
            for (int i = 'A'; i <= 'Z'; i++)
            {
                if (i == 'I' || i == 'O')
                {
                    continue;
                }
                g_ucLocalCity[nLocalCityIndex++] = (byte)i;
                comboBoxCity.Items.Add(string.Format("{0}", (Char)i));
            }
            g_ucLocalCity[nLocalCityIndex++] = 0;
            comboBoxCity.Items.Add("全省");
            comboBoxCity.Text = "全省";
        }

        private void Demo_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (nCamId != -1)
            {
                MyClass.Net_StopVideo(nCamId);
                MyClass.Net_DisConnCamera(nCamId);
                MyClass.Net_DelCamera(nCamId);
                nCamId = -1;
            }
            MyClass.Net_UNinit();
        }
        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Connect_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            comboBoxIP.Items.Clear();
            comboBoxIP.Sorted = true;
            MyClass.Net_FindDevice(fNetFindDeviceCallback, IntPtr.Zero);
            comboBoxIP.DroppedDown = true;
            Cursor.Current = Cursors.Default;

            do 
            {
                if (comboBoxIP.Text == "")
                {
                    MessageBox.Show("请输入相机IP!", "提示");
                    break;
                }

                nCamId = MyClass.Net_AddCamera(comboBoxIP.Text);
                if (nCamId == -1)
                {
                    MessageBox.Show("添加相机失败!", "提示");
                    break;
                }

                int iRet = MyClass.Net_ConnCamera(nCamId, 30000, 10);
                if (iRet != 0)
                {
                    MyClass.Net_DelCamera(nCamId);
                    MessageBox.Show("连接相机失败!", "提示");
                    break;
                }

                iRet = MyClass.Net_StartVideo(nCamId, 0, tabPageVideo.Handle);
                if (iRet != 0)
                {
                    MyClass.Net_DisConnCamera(nCamId);
                    MyClass.Net_DelCamera(nCamId);
                    MessageBox.Show("打开视频失败!", "提示");
                    break;
                }

                MyClass.Net_RegOffLineClient(nCamId);

                // if (fGetImageCB == null)
                //fGetImageCB = new MyClass.FGetImageCB(FGetImageCB);
                //MyClass.Net_RegImageRecv(fGetImageCB);
                if (fGetImageCB2 == null)
                    fGetImageCB2 = new MyClass.FGetImageCB2(FGetImageCB2);
                MyClass.Net_RegImageRecv2(fGetImageCB2);

                if (fGetOffLinePayRecordCB == null)
                    fGetOffLinePayRecordCB = new MyClass.FGetOffLinePayRecordCB(FGetOffLinePayRecordCB);
                MyClass.Net_RegOffLinePayRecord(nCamId, fGetOffLinePayRecordCB, IntPtr.Zero);

                if (fGetOffLineImageCBEx == null)
                    fGetOffLineImageCBEx = new MyClass.FGetOffLineImageCBEx(FGetOffLineImageCBEx);
                MyClass.Net_RegOffLineImageRecvEx(nCamId, fGetOffLineImageCBEx, IntPtr.Zero);

                //if (fGetReportCBEx == null)
                   // fGetReportCBEx = new MyClass.FGetReportCBEx(FGetReportCBEx);
                MyClass.Net_RegReportMessEx(nCamId, fGetReportCBEx, IntPtr.Zero);

                iRet = MyClass.Net_QueryVideoDetectSetup(nCamId, ref tVideoDetectParamSetup);
                if (iRet != 0)
                {
                    //MessageBox.Show("查询视频检测区域参数失败!", "提示");
                }
                else
                {
                    textBoxPlateLeftUpX.Text = tVideoDetectParamSetup.atPlateRegion[0].sX.ToString();
                    textBoxPlateLeftUpY.Text = tVideoDetectParamSetup.atPlateRegion[0].sY.ToString();
                    textBoxPlateRightDownX.Text = tVideoDetectParamSetup.atPlateRegion[2].sX.ToString();
                    textBoxPlateRightDownY.Text = tVideoDetectParamSetup.atPlateRegion[2].sY.ToString();
                }

                buttonVehicleVAFunQuery_Click(sender, e);

            } while (false);

            Cursor.Current = Cursors.Default;
        }
        /// <summary>
        /// 断开相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_DisConnect_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (nCamId != -1)
            {
                MyClass.Net_StopVideo(nCamId);
                MyClass.Net_DisConnCamera(nCamId);
                MyClass.Net_DelCamera(nCamId);
                nCamId = -1;
            }
            Cursor.Current = Cursors.Default;
        }
        /// <summary>
        ///  相机抓拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGrab_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            MyClass.T_FrameInfo tFrameInfo = new MyClass.T_FrameInfo();

            int iRet = MyClass.Net_ImageSnap(nCamId, ref tFrameInfo);
            if (iRet == 0)
            {
                MessageBox.Show("抓拍成功", "提示");
            }
            else
            {
                MessageBox.Show("抓拍失败", "提示");
            }
        }
        /// <summary>
        ///  相机抬闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGate_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            MyClass.T_ControlGate tControlGate = new MyClass.T_ControlGate();
            tControlGate.ucState = 1;
            tControlGate.ucReserved = new byte[3]{ 0,0,0};

            int iRet = MyClass.Net_GateSetup(nCamId, ref tControlGate);
            if (iRet == 0)
            {
                MessageBox.Show("抬闸成功", "提示");
            }
            else
            {
                MessageBox.Show("抬闸失败", "提示");
            }
        }

        

        

        private void buttonResetPlate_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            MyClass.T_LprResult tLprResult = new MyClass.T_LprResult();
            byte[] LprResult = System.Text.Encoding.Default.GetBytes(textBoxPlate.Text);
            tLprResult.LprResult = new byte[16];
            LprResult.CopyTo(tLprResult.LprResult, 0);

            byte[] StartTime = System.Text.Encoding.Default.GetBytes(dateTimePickerStart.Value.ToString("yyyyMMddHHmmss"));
            tLprResult.StartTime = new byte[16];
            StartTime.CopyTo(tLprResult.StartTime, 0);

            byte[] EndTime = System.Text.Encoding.Default.GetBytes(dateTimePickerEnd.Value.ToString("yyyyMMddHHmmss"));
            tLprResult.EndTime = new byte[16];
            EndTime.CopyTo(tLprResult.EndTime, 0);

            MyClass.T_BlackWhiteListCount tBlackWhiteListCount = new MyClass.T_BlackWhiteListCount();
            tBlackWhiteListCount.uiCount = 1;
            string strAucLplPath = System.Windows.Forms.Application.StartupPath + "\\lpr.ini";
            if (File.Exists(strAucLplPath))
            {
                File.Delete(strAucLplPath);
            }
            byte[] aucLplPath = System.Text.Encoding.Default.GetBytes(strAucLplPath);
            tBlackWhiteListCount.aucLplPath = new byte[256];
            aucLplPath.CopyTo(tBlackWhiteListCount.aucLplPath, 0);

            int iRet = MyClass.Net_BlackWhiteListSetup(ref tLprResult, ref tBlackWhiteListCount);
            if (iRet == 0)
            {

            }
            else
            {
                MessageBox.Show("生成白名单失败", "提示");
            }

            MyClass.T_BlackWhiteList tBalckWhiteList = new MyClass.T_BlackWhiteList();
            tBalckWhiteList.LprMode = 1;
            tBalckWhiteList.Lprnew = 0;
            tBalckWhiteList.aucLplPath = new byte[256];
            aucLplPath.CopyTo(tBalckWhiteList.aucLplPath, 0);

            iRet = MyClass.Net_BlackWhiteListSend(nCamId, ref tBalckWhiteList);
            if (iRet == 0)
            {
                MessageBox.Show("重设白名单成功", "提示");
            }
            else
            {
                MessageBox.Show("重设白名单失败", "提示");
            }
        }
        private void buttonAddPlate_Click(object sender, EventArgs e)
        {
            /*
             if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }
             */


            // 初始化数据
            PlateBO plateBO = new PlateBO();
            plateBO.id = Utils.sid();
            plateBO.plate = textBoxPlate.Text;
            plateBO.department = textBoxDepartment.Text;
            plateBO.name = textBoxName.Text;
            plateBO.phone = textBoxPhone.Text;

            // 新增数据
            MysqlUtils mysqlUtils = new MysqlUtils();
            DialogResult dg = MessageBox.Show("确定新增吗？", "新增", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dg == DialogResult.Yes)
            {
                //...
                mysqlUtils.Insert(plateBO);
            }
            else { return; }
            MessageBox.Show("新增车牌成功！");
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            MyClass.T_LprResult tLprResult = new MyClass.T_LprResult();
            byte[] LprResult = System.Text.Encoding.Default.GetBytes(textBoxPlate.Text);
            tLprResult.LprResult = new byte[16];
            LprResult.CopyTo(tLprResult.LprResult, 0);

            byte[] StartTime = System.Text.Encoding.Default.GetBytes(dateTimePickerStart.Value.ToString("yyyyMMddHHmmss"));
            tLprResult.StartTime = new byte[16];
            StartTime.CopyTo(tLprResult.StartTime, 0);

            byte[] EndTime = System.Text.Encoding.Default.GetBytes(dateTimePickerEnd.Value.ToString("yyyyMMddHHmmss"));
            tLprResult.EndTime = new byte[16];
            EndTime.CopyTo(tLprResult.EndTime, 0);

            MyClass.T_BlackWhiteListCount tBlackWhiteListCount = new MyClass.T_BlackWhiteListCount();
            tBlackWhiteListCount.uiCount = 1;
            string strAucLplPath = System.Windows.Forms.Application.StartupPath + "\\lpr.ini";
            if (File.Exists(strAucLplPath))
            {
                File.Delete(strAucLplPath);
            }
            byte[] aucLplPath = System.Text.Encoding.Default.GetBytes(strAucLplPath);
            tBlackWhiteListCount.aucLplPath = new byte[256];
            aucLplPath.CopyTo(tBlackWhiteListCount.aucLplPath, 0);

            int iRet = MyClass.Net_BlackWhiteListSetup(ref tLprResult, ref tBlackWhiteListCount);
            if (iRet == 0)
            {

            }
            else
            {
                MessageBox.Show("生成白名单失败", "提示");
            }

            MyClass.T_BlackWhiteList tBalckWhiteList = new MyClass.T_BlackWhiteList();
            tBalckWhiteList.LprMode = 1;
            tBalckWhiteList.Lprnew = 1;
            tBalckWhiteList.aucLplPath = new byte[256];
            aucLplPath.CopyTo(tBalckWhiteList.aucLplPath, 0);

            iRet = MyClass.Net_BlackWhiteListSend(nCamId, ref tBalckWhiteList);
            if (iRet == 0)
            {
                MessageBox.Show("新增白名单成功", "提示");
            }
            else
            {
                MessageBox.Show("新增白名单失败", "提示");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string plate =  textBoxPlate.Text;
            MysqlUtils mysqlUtils = new MysqlUtils();
            DialogResult dg = MessageBox.Show("确定删除吗？", "删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dg == DialogResult.Yes)
            {
                //...
                mysqlUtils.Delete(plate);
            }
            MessageBox.Show("车牌删除成功！");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            MyClass.T_LprResult tLprResult = new MyClass.T_LprResult();
            byte[] LprResult = System.Text.Encoding.Default.GetBytes(textBoxPlate.Text);
            tLprResult.LprResult = new byte[16];
            LprResult.CopyTo(tLprResult.LprResult, 0);

            byte[] StartTime = System.Text.Encoding.Default.GetBytes(dateTimePickerStart.Value.ToString("yyyyMMddHHmmss"));
            tLprResult.StartTime = new byte[16];
            StartTime.CopyTo(tLprResult.StartTime, 0);

            byte[] EndTime = System.Text.Encoding.Default.GetBytes(dateTimePickerEnd.Value.ToString("yyyyMMddHHmmss"));
            tLprResult.EndTime = new byte[16];
            EndTime.CopyTo(tLprResult.EndTime, 0);

            MyClass.T_BlackWhiteListCount tBlackWhiteListCount = new MyClass.T_BlackWhiteListCount();
            tBlackWhiteListCount.uiCount = 1;
            string strAucLplPath = System.Windows.Forms.Application.StartupPath + "\\lpr.ini";
            if (File.Exists(strAucLplPath))
            {
                File.Delete(strAucLplPath);
            }

            byte[] aucLplPath = System.Text.Encoding.Default.GetBytes(strAucLplPath);
            tBlackWhiteListCount.aucLplPath = new byte[256];
            aucLplPath.CopyTo(tBlackWhiteListCount.aucLplPath, 0);

            int iRet = MyClass.Net_BlackWhiteListSetup(ref tLprResult, ref tBlackWhiteListCount);
            if (iRet == 0)
            {
                //MessageBox.Show("生成白名单成功", "提示");
                Process.Start("Notepad.exe", strAucLplPath);
            }
            else
            {
                MessageBox.Show("生成白名单失败", "提示");
            }
        }
        /// <summary>
        ///  手动抓图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAutoGrab_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            //////////////////////////////////////////////////////////////////////////////
            //直接保存为JPG图片
            //string strTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmssfff");
            //string strJpgPath = strTime + ".jpg";
            //int iRet = MyClass.Net_SaveJpgFile(nCamId, strJpgPath);
            //if (iRet == 0)
            //{
            //    //MessageBox.Show("手动抓图成功", "提示");
            //    if (File.Exists(strJpgPath))
            //    {
            //         Process.Start("Explorer.exe", strJpgPath);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("手动抓图失败", "提示");
            //}

            ////////////////////////////////////////////////////////////////////////
            //获取当前帧的JPG缓存
            IntPtr ucJpgBuffer = IntPtr.Zero; 
            ulong ulSize = 0;

            int iRet = MyClass.Net_GetJpgBuffer(nCamId, ref ucJpgBuffer, ref ulSize);

            if (iRet == 0)
            {
                //将当前帧的JPG缓存刷到控件上
                byte[] ByteJpgBuffer = new byte[ulSize];
                Marshal.Copy(ucJpgBuffer, ByteJpgBuffer, 0, (int)ulSize);
                MemoryStream ms = new MemoryStream(ByteJpgBuffer);
                pictureBoxPlate.Image = Image.FromStream(ms);


                //////////////////////////////////////////////////////////////////////
                //将缓存保存为JPG图片
                //string strTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmssfff");
                //string strJpgPath = strTime + ".jpg";
                //FileStream fs = new FileStream(strJpgPath, FileMode.Create, FileAccess.Write | FileAccess.Read, FileShare.None);
                //fs.Write(ByteJpgBuffer, 0, (int)ulSize);
                //fs.Close();
                //fs.Dispose();

                //if (File.Exists(strJpgPath))
                //{
                //    Process.Start("Explorer.exe", strJpgPath);
                //}
            }
            else
            {
                MessageBox.Show("手动抓图失败", "提示");
            }

            if (ucJpgBuffer != IntPtr.Zero)
            {
                MyClass.Net_FreeBuffer(ucJpgBuffer);
                ucJpgBuffer = IntPtr.Zero;
            }

        }
        /// <summary>
        /// 修改ip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonModifyIp_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            ModifyIP dlgModifyIP = new ModifyIP();
            dlgModifyIP.nCamId = nCamId;
            DialogResult dr = dlgModifyIP.ShowDialog();

            if (DialogResult.OK == dr)
            {
                comboBoxIP.Text = dlgModifyIP.strIp;

                button_DisConnect_Click(sender, e);
                button_Connect_Click(sender, e);
            }
        }

        private void buttonEncpyptionRead_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            int ptLen = 256; //长度需要大于256
            StringBuilder strEncpyption = new StringBuilder(ptLen);
            int iRet = MyClass.Net_ReadTwoEncpyption(nCamId, strEncpyption, (uint)ptLen);
            if (iRet == -1)
            {
                MessageBox.Show("读取加密数据失败", "提示");
                return;
            }

            textBoxEncpyption.Text = strEncpyption.ToString();
        }

        private void buttonEncpyptionWrite_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            Byte[] byteUnicode = Encoding.Unicode.GetBytes(textBoxEncpyption.Text);
            //统计Ansi字节数
            string strAnsi = Encoding.ASCII.GetString(byteUnicode);
            uint nLenAnsi = (uint)strAnsi.Length;

            int iRet = MyClass.Net_WriteTwoEncpyption(nCamId, textBoxEncpyption.Text, nLenAnsi);
            if (iRet == 0)
            {
                MessageBox.Show("设置加密数据成功", "提示");
            }
            else
            {
                MessageBox.Show("设置加密数据失败", "提示");
            }
        }

        private void buttonPlateSet_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            int nPlateLeftUpX = int.Parse(textBoxPlateLeftUpX.Text);
            int nPlateLeftUpY = int.Parse(textBoxPlateLeftUpY.Text);
            int nPlateRightDownX = int.Parse(textBoxPlateRightDownX.Text);
            int nPlateRightDownY = int.Parse(textBoxPlateRightDownY.Text);

            tVideoDetectParamSetup.atPlateRegion[0].sX = (short)(nPlateLeftUpX);
            tVideoDetectParamSetup.atPlateRegion[0].sY = (short)(nPlateLeftUpY);
            tVideoDetectParamSetup.atPlateRegion[1].sX = (short)(nPlateLeftUpX);
            tVideoDetectParamSetup.atPlateRegion[1].sY = (short)(nPlateRightDownY);
            tVideoDetectParamSetup.atPlateRegion[2].sX = (short)(nPlateRightDownX);
            tVideoDetectParamSetup.atPlateRegion[2].sY = (short)(nPlateRightDownY);
            tVideoDetectParamSetup.atPlateRegion[3].sX = (short)(nPlateRightDownX);
            tVideoDetectParamSetup.atPlateRegion[3].sY = (short)(nPlateLeftUpY);

            int iRet = MyClass.Net_VideoDetectSetup(nCamId, ref tVideoDetectParamSetup);
            if (iRet != 0)
            {
                MessageBox.Show("设置视频检测区域参数失败!", "提示");
            }

            MyClass.Net_UpdatePlateRegion(nCamId);
        }

        private void buttonRs485Send_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            string[] hexValuesSplit = textBoxRs485Data.Text.TrimStart().TrimEnd().Split(' ');

            tRS485Data.rs485Id = 0;
            tRS485Data.dataLen = 0;
            tRS485Data.data = new byte [1024];

            foreach (String hex in hexValuesSplit)
            {
                tRS485Data.data[tRS485Data.dataLen++] = (byte)Convert.ToInt32(hex, 16);
            }

            int iRet = MyClass.Net_SendRS485Data(nCamId, ref tRS485Data);
            if (iRet != 0)
            {
                MessageBox.Show("发送Rs485数据失败!", "提示");
            }
            else
            {
                MessageBox.Show("发送Rs485数据成功!", "提示");
            }
        }

        private void checkBoxShowPlateRegion_CheckedChanged(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            if (checkBoxShowPlateRegion.Checked)
            {
                MyClass.Net_ShowPlateRegion(nCamId, 1);
            }
            else
            {
                MyClass.Net_ShowPlateRegion(nCamId, 0);
            }
        }

        private void buttonTimeRead_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            int iRet = MyClass.Net_QueryTimeSetup(nCamId, ref tDCTimeSetup);
            if (iRet != 0)
            {
                MessageBox.Show("查询相机时间失败!", "提示");
                return;
            }

            dateTimePickerCAM.Value = new DateTime(tDCTimeSetup.usYear, tDCTimeSetup.ucMonth, tDCTimeSetup.ucDay,
                                                   tDCTimeSetup.ucHour, tDCTimeSetup.ucMinute, tDCTimeSetup.ucSecond);
        }

        private void buttonTimeWrite_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            tDCTimeSetup.usYear = (ushort)dateTimePickerCAM.Value.Year;
            tDCTimeSetup.ucMonth = (byte)dateTimePickerCAM.Value.Month;
            tDCTimeSetup.ucDay = (byte)dateTimePickerCAM.Value.Day;

            tDCTimeSetup.ucHour = (byte)dateTimePickerCAM.Value.Hour;
            tDCTimeSetup.ucMinute = (byte)dateTimePickerCAM.Value.Minute;
            tDCTimeSetup.ucSecond = (byte)dateTimePickerCAM.Value.Second;

            int iRet = MyClass.Net_TimeSetup(nCamId, ref tDCTimeSetup);
            if (iRet != 0)
            {
                MessageBox.Show("设置相机时间失败!", "提示");
                return;
            }
        }

        private void buttonVehicleVAFunSetup_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            tVehicleVAFunSetup.uiPlateDefaultWord = g_uiPlateDefaultWord[comboBoxProvince.SelectedIndex];
            tVehicleVAFunSetup.ucLocalCity = g_ucLocalCity[comboBoxCity.SelectedIndex];

            int iRet = MyClass.Net_VehicleVAFunSetup(nCamId, ref tVehicleVAFunSetup);
            if (iRet != 0)
            {
                MessageBox.Show("设置相机默认字失败!", "提示");
                return;
            }
        }

        private void buttonVehicleVAFunQuery_Click(object sender, EventArgs e)
        {
            /*
             if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }
             */
            // 数据初始化
            string province = comboBoxProvince.Text;
            string area = comboBoxCity.Text;
            string plateTail = textBoxPlateTail.Text;
            string plate = province + area + plateTail;// 车牌拼接
            PlateBO plateBO = new PlateBO();
            plateBO.plate = plate;
            MysqlUtils mysqlUtils = new MysqlUtils();
            List<string> carWashRocordList = mysqlUtils.PriciseSelectOO(plateBO);

            foreach(var item  in carWashRocordList){
                Console.WriteLine(item);
            }

            // 将查找出的string转为对象
            List<PlateBO> plateBoList = new List<PlateBO>();
            foreach(var strItem in carWashRocordList){
                string[] str = strItem.Split(',');// 字符串转为数组
                PlateBO plateBOTemp = new PlateBO();
                plateBOTemp.id = str[0];
                plateBOTemp.plate = str[1];
                plateBOTemp.department = str[2];
                plateBOTemp.name = str[3];
                plateBOTemp.phone = str[4];
                plateBOTemp.carWash = Convert.ToInt32(str[5]);
                plateBOTemp.createTime = str[6];
                plateBoList.Add(plateBOTemp);
            }


            listViewCarWashRecord.Items.Clear();// 遍历泛型集合前先清除ListView.Items中的数据，以免每次点击刷新时，都会将数据库中数据放入泛型集合中，使泛型集合重复添加元素，越来越大
            //★★★
            foreach (PlateBO item in plateBoList)//遍历查询出来的数据
            {
                ListViewItem lv = new ListViewItem();
                lv.Text = item.plate;//设置第一行显示的数据
                //绑定剩余列的数据
                lv.SubItems.Add(item.department);
                lv.SubItems.Add(item.name);
                lv.SubItems.Add(item.phone);
                lv.SubItems.Add(Convert.ToString(item.carWash));
                lv.SubItems.Add(item.createTime);
                //行数据创建完毕后添加到列表中
                listViewCarWashRecord.Items.Add(lv);
            }        
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            comboBoxIP.Items.Clear();
            comboBoxIP.Sorted = true;
            MyClass.Net_FindDevice(fNetFindDeviceCallback, IntPtr.Zero);
            comboBoxIP.DroppedDown = true;
            Cursor.Current = Cursors.Default;
        }

        private static bool m_bTalking = false;
        /*
         * private void buttonTalk_Click(object sender, EventArgs e)
        {
            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }

            int iRet = 0;

            if (false == m_bTalking)
            {
                iRet = MyClass.Net_StartTalk(nCamId);

                if (0 == iRet)
                {
                    m_bTalking = true;
                    buttonTalk.Text = "结束对讲";
                    MessageBox.Show("开始对讲成功", "提示");
                }
                else
                {
                    MessageBox.Show("开始对讲失败", "提示");
                }
            }
            else
            {
                iRet = MyClass.Net_StopTalk(nCamId);

                if (0 == iRet)
                {
                    m_bTalking = false;
                    buttonTalk.Text = "开始对讲";
                    MessageBox.Show("结束对讲成功", "提示");
                }
                else
                {
                    MessageBox.Show("结束对讲失败", "提示");
                   
                }
            }
        }
         */

        private void btn_SendBMess_Click(object sender, EventArgs e)
        {

            if (nCamId == -1)
            {
                MessageBox.Show("相机未连接", "提示");
                return;
            }
            
            string strPlate = textBoxPlate.Text.Trim();
            if (strPlate.Length == 0)
            {
                MessageBox.Show("要下发的单个车牌不能为空", "参数错误");
                return;
            }
            if (strPlate.Length > 16)
            {
                MessageBox.Show("测试工程只做单个车牌下发", "参数错误");
                return;
            }

            MyClass.T_SendLprByMess tSendLprByMess = new MyClass.T_SendLprByMess();
            tSendLprByMess.aucReserved = new byte[2];
            tSendLprByMess.atLprResult = new MyClass.T_LprResult[10];
            tSendLprByMess.ucType = 0;
            tSendLprByMess.ucConut = 1;
            byte[] LprResult = System.Text.Encoding.Default.GetBytes(strPlate);
            tSendLprByMess.atLprResult[0].LprResult = new byte[16];
            LprResult.CopyTo(tSendLprByMess.atLprResult[0].LprResult, 0);


            byte[] StartTime = System.Text.Encoding.Default.GetBytes(dateTimePickerStart.Value.ToString("yyyyMMddHHmmss"));
            tSendLprByMess.atLprResult[0].StartTime = new byte[16];
            StartTime.CopyTo(tSendLprByMess.atLprResult[0].StartTime, 0);

            byte[] EndTime = System.Text.Encoding.Default.GetBytes(dateTimePickerEnd.Value.ToString("yyyyMMddHHmmss"));
            tSendLprByMess.atLprResult[0].EndTime = new byte[16];
            EndTime.CopyTo(tSendLprByMess.atLprResult[0].EndTime, 0);

            int iRet = MyClass.Net_SendBlackWhiteListByMess(nCamId, ref tSendLprByMess);
            if (iRet != 0)
            {
                MessageBox.Show("单条车牌下发失败!", "提示");
                //return;
            }
            else
            {
                MessageBox.Show("单条车牌下发成功!", "提示");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MyClass.T_AudioLinkinfo tAudioLinkinfo = new MyClass.T_AudioLinkinfo();
            int iRet = MyClass.Net_QueryAudioLinkinfo(nCamId, ref tAudioLinkinfo);
            if (iRet != 0)
            {
                MessageBox.Show("查询相机对讲状态失败!", "提示");
                //return;
            }
            else
            {
                if(tAudioLinkinfo.ucStatus == 1)
                    MessageBox.Show("已被占用!", "提示");
                else
                    MessageBox.Show("空闲!", "提示");

            }
        }

        private void buttonFindCarWashRecord_Click(object sender, EventArgs e)
        {
            // 数据初始化
            string province = comboBoxProvince.Text;
            string area = comboBoxCity.Text;
            string plateTail = textBoxPlateTail.Text;
            string plate = province + area + plateTail;// 车牌拼接
            WashRecordBO washRecordBO = new WashRecordBO();
            washRecordBO.plate = plate;
            MysqlUtils mysqlUtils = new MysqlUtils();
            List<string> carWashRocordList = mysqlUtils.PriciseSelectCarWashRecord(washRecordBO);

            foreach (var item in carWashRocordList)
            {
                Console.WriteLine(item);
            }

            // 将查找出的string转为对象
            List<WashRecordBO> washRecordBOList = new List<WashRecordBO>();
            foreach (var strItem in carWashRocordList)
            {
                string[] str = strItem.Split(',');// 字符串转为数组
                WashRecordBO washRecordBOTemp = new WashRecordBO();
                washRecordBOTemp.id = str[0];
                washRecordBOTemp.plate = str[1];
                washRecordBOTemp.washTime = str[2];
                washRecordBOList.Add(washRecordBOTemp);
            }


            listViewCarWashRecord.Items.Clear();// 遍历泛型集合前先清除ListView.Items中的数据，以免每次点击刷新时，都会将数据库中数据放入泛型集合中，使泛型集合重复添加元素，越来越大
            //★★★
            foreach (WashRecordBO item in washRecordBOList)//遍历查询出来的数据
            {
                ListViewItem lv = new ListViewItem();
                lv.Text = item.plate;//设置第一行显示的数据
                //绑定剩余列的数据
                lv.SubItems.Add("");
                lv.SubItems.Add("");
                lv.SubItems.Add("");
                lv.SubItems.Add("");
                lv.SubItems.Add(item.washTime);
                //行数据创建完毕后添加到列表中
                listViewCarWashRecord.Items.Add(lv);
            }        
        }

        private void buttonUpdatePlate_Click(object sender, EventArgs e)
        {
            // 1. 先根据人名、车牌查询出id
            string plate = textBoxPlate.Text;
            MysqlUtils mysqlUtils = new MysqlUtils();
            string result = mysqlUtils.PriciseSelectByPlate(plate);

            PlateBO plateBO = new PlateBO();
            string[] str = result.Split(',');// 字符串转为数组
            plateBO.id = str[0];// 获取id
            plateBO.plate = textBoxPlateNew.Text;
            if (textBoxDepartment.Text == "")// 若空，默认原值
            {
                plateBO.department = str[2];
            }
            else
            {
                plateBO.department = textBoxDepartment.Text;
            }

            if (textBoxName.Text == "")// 若空，默认原值
            {
                plateBO.name = str[3];
            }
            else
            {
                plateBO.name = textBoxName.Text;
            }

            if (textBoxPhone.Text == "")// 若空，默认原值
            {
                plateBO.phone = str[4];
            }
            else
            {
                plateBO.phone = textBoxPhone.Text;
            }

            if (textBoxPlateNew.Text == "")
            {
                MessageBox.Show("请输入新车牌");
            }
           
            // 2. 根据id更改车主信息
            DialogResult dg = MessageBox.Show("确定修改吗？", "修改", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dg == DialogResult.Yes)
            {
                //...
                mysqlUtils.UpdateCarInfo(plateBO);
            }
            else { return; }
            MessageBox.Show("车牌修改成功！");
        }

        private void buttonFuzzySearch_Click(object sender, EventArgs e)
        {
            // 数据初始化

            string plate= textBoxPlateTail.Text;

            PlateBO plateBO = new PlateBO();
            plateBO.plate = plate;
            MysqlUtils mysqlUtils = new MysqlUtils();
            // TODO
            List<string> carWashRocordList = mysqlUtils.FuzzySelectOO(plateBO);

            foreach (var item in carWashRocordList)
            {
                Console.WriteLine(item);
            }

            // 将查找出的string转为对象
            List<PlateBO> plateBoList = new List<PlateBO>();
            foreach (var strItem in carWashRocordList)
            {
                string[] str = strItem.Split(',');// 字符串转为数组
                PlateBO plateBOTemp = new PlateBO();
                plateBOTemp.id = str[0];
                plateBOTemp.plate = str[1];
                plateBOTemp.department = str[2];
                plateBOTemp.name = str[3];
                plateBOTemp.phone = str[4];
                plateBOTemp.carWash = Convert.ToInt32(str[5]);
                plateBOTemp.createTime = str[6];
                plateBoList.Add(plateBOTemp);
            }


            listViewCarWashRecord.Items.Clear();// 遍历泛型集合前先清除ListView.Items中的数据，以免每次点击刷新时，都会将数据库中数据放入泛型集合中，使泛型集合重复添加元素，越来越大
            //★★★
            foreach (PlateBO item in plateBoList)//遍历查询出来的数据
            {
                ListViewItem lv = new ListViewItem();
                lv.Text = item.plate;//设置第一行显示的数据
                //绑定剩余列的数据
                lv.SubItems.Add(item.department);
                lv.SubItems.Add(item.name);
                lv.SubItems.Add(item.phone);
                lv.SubItems.Add(Convert.ToString(item.carWash));
                lv.SubItems.Add(item.createTime);
                //行数据创建完毕后添加到列表中
                listViewCarWashRecord.Items.Add(lv);
            }     
        }
    }
}