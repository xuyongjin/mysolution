using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunRise.HOSP.Resources;
using SunRise.HOSP.Hardware;

namespace SunRise.HOSP.CLIENT
{
    public class ReadCard
    {
        /// <summary>
        /// 读卡器
        /// </summary>
        private IReadCardInterface m_ReadCard = null;

        /// <summary>
        /// 社保读卡器
        /// </summary>
        private ISocialSecurity m_SB = null;
        /// <summary>
        /// 读卡器类型
        /// </summary>
        public static READ_CARD_TYPE ReadCardType { get; set; }
        /// <summary>
        /// 社保读卡类型
        /// </summary>
        public static SocialSecurityType SocialSecurityType { get; set; }
        private bool m_bUseLight { get; set; }

        public ReadCard(bool bTest = false,bool bUseLight = true)
        {
            m_ReadCard = ReadCardInstance.GetReadCardInstance(ReadCardType, bTest);
            m_bUseLight = bUseLight;
          //  m_bUseLight = false;//test
            if (m_ReadCard == null)
            {
                throw new Exception("设置读卡器失败，请检查类型");
            }

            m_SB = ReadCardInstance.GetSocialSecurityInstance(SocialSecurityType, bTest);
            if (m_SB == null)
            {
                throw new Exception("设置社保读卡失败，请检查类型");
            }
        }

        #region 打开关闭
        /// <summary>
        /// 打开端口 
        /// </summary>
        /// <param name="Port">空则读取默认配置</param>
        /// <param name="Baudrate"></param>
        /// <returns></returns>
        public bool Open(string Port, UInt32 Baudrate = 9600)
        {
            if (m_bUseLight)
            {
                HOSP.Hardware.DoorLight.GetInstance().LightGlint(HOSP.Resources.Config.LightReadCardPos, true);
            }
            return (m_ReadCard.Open(Port, Baudrate));
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        public bool CloseComm()
        {
            return (m_ReadCard.CloseComm() );
        } 
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            return m_ReadCard.Initialize();
        }

        #region 允许进卡

        /// <summary>
        /// 允许进卡
        /// 
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AllowedCardIn()
        {
            if (m_bUseLight)
            {
                HOSP.Hardware.DoorLight.GetInstance().LightGlint(HOSP.Resources.Config.LightReadCardPos, true);
            }
            System.Threading.Thread.Sleep(200);
            return m_ReadCard.AllowCardIn(AllowFrontCardIn.CanIn);
        }

        /// <summary>
        /// 禁止进卡
        /// </summary>
        public bool NotAllowedCardIn()
        {
            if (m_bUseLight)
            {
                HOSP.Hardware.DoorLight.GetInstance().LightOn(HOSP.Resources.Config.LightReadCardPos, true);
            }
            System.Threading.Thread.Sleep(200);
            return m_ReadCard.AllowCardIn(AllowFrontCardIn.NotIn);
        }

        #endregion

        #region 移卡

        /// <summary>
        /// 移到前端
        /// </summary>
        public bool MoveCardToFront()
        {
            System.Threading.Thread.Sleep(200);
            if (m_bUseLight )
            {
                HOSP.Hardware.DoorLight.GetInstance().LightOn(HOSP.Resources.Config.LightReadCardPos, true);
            }
            return m_ReadCard.MoveCard(MoveCardPosition.MoveToFront);
        }

        /// <summary>
        /// 移到前端 持卡
        /// </summary>
        public bool MoveCardToFront_Hold()
        {
            System.Threading.Thread.Sleep(200);
            if (m_bUseLight)
            {
                HOSP.Hardware.DoorLight.GetInstance().LightOn(HOSP.Resources.Config.LightReadCardPos, true);
            }
            return m_ReadCard.MoveCard(MoveCardPosition.MoveToFront_Hold);
        }

        /// <summary>
        /// 移到RF卡
        /// </summary>
        public bool MoveCardToRF()
        {
            return m_ReadCard.MoveCard(MoveCardPosition.MoveToRF);
        }

        /// <summary>
        /// 移到IC卡
        /// </summary>
        public bool MoveCardToIC()
        {
            return m_ReadCard.MoveCard(MoveCardPosition.MoveToIC);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveCardToRecycle()
        {
            return m_ReadCard.MoveCard(MoveCardPosition.MoveToRecycle);
        }

        /// <summary>
        /// 移到磁卡位
        /// </summary>
        public bool MoveCardToMagstrip()
        {
            return m_ReadCard.MoveCard(MoveCardPosition.MoveToMAGSTRIP);
        }

        #endregion

        /// <summary>
        /// 允许移到到IC卡拉
        /// </summary>
        /// <returns></returns>
        public bool SetDockIcPos()
        {
            return m_ReadCard.SetDockIcPos();
        }


        public bool GetStatus(out ChannelStatus cs, out BoxStatus bs, out RecycleStatus rs)
        {
            return m_ReadCard.GetStatus(out cs, out bs, out rs);
        }

        /////////////////////////////////////////////////////////////////////
        //社保 
        /////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 测试卡是否插入卡槽
        /// </summary>
        /// <returns></returns>
        public bool TestCardInsert()
        {
            ChannelStatus cs; BoxStatus bs; RecycleStatus rs;
            if (GetStatus(out cs, out bs, out rs))
            {
                //通道里有卡
                if (cs == ChannelStatus.ChannelHaveCard)
                {
                    if (m_bUseLight)
                    {
                        HOSP.Hardware.DoorLight.GetInstance().LightOn(HOSP.Resources.Config.LightReadCardPos, true);
                    } 
                    //MoveCardToIC();
                    return true;
                }
            }
            
            return false;
            //return m_SB.TestCardInsert();
        }


        /// <summary>
        /// 读取卡号
        /// </summary>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ReadICCardNo(ref string CardNo)
        {
            return true;
        }

        /// <summary>
        /// 读取卡数据
        /// </summary>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ReadMagCardNo(ref string CardNo)
        {
            MagcardData data = new MagcardData();
            if (Config.HospCode.Equals(HospCode.NcDaXueDIErFuShuYiYuan))
            {
                data.GetTrack1 = true;
            }
            else
            {
                data.GetTrack2 = true;
            }
           
            
            if (m_ReadCard.ReadMagcardData(data))
            {
                if (Config.HospCode.Equals(HospCode.NcDaXueDIErFuShuYiYuan))
                {
                    CardNo = data.Track1Data;
                }
                else
                {
                    CardNo = data.Track2Data;
                }
               
                return true;
            }
            return false;
        }


        /// <summary>
        /// 读取卡数据
        /// </summary>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ReadMagCardNo(ref string CardNo, int readTrack)
        {
            MagcardData data = new MagcardData();
           if(readTrack==1)
           {
                data.GetTrack1 = true;
            }
           else if (readTrack == 2)
            {
                data.GetTrack2 = true;
            }
           else if (readTrack == 3)
           {
               data.GetTrack3 = true;
           }
           else
           {
               data.GetTrack2 = true;
           }


            if (m_ReadCard.ReadMagcardData(data))
            {
                if (readTrack == 1)
                {
                    CardNo = data.Track1Data;
                }
                else if (readTrack == 2)
                {
                    CardNo = data.Track2Data;
                }
                else if (readTrack == 3)
                {
                    CardNo = data.Track3Data;
                }
                else
                {
                    CardNo = data.Track2Data;
                }
                //if (Config.HospCode.Equals(HospCode.NcDaXueDIErFuShuYiYuan))
                //{
                //    CardNo = data.Track1Data;
                //}
                //else
                //{
                //    CardNo = data.Track2Data;
                //}

                return true;
            }
            return false;
        }

        /// <summary>
        /// 读取状态
        /// 限制卡号位数，卡号长度等于JZKCardCount时 为健康卡 ，等于28位时，是磁条社保卡
        /// 否则为芯片卡
        /// </summary>
        /// <param name="CardType"></param>
        /// <returns></returns>
        public bool ReadCardNo(ref InsertCardType card, ref string CardNo)
        {

            if (ReadMagCardNo(ref CardNo))
            {
                //if (CardNo.Length ==12)
                //{
                //    card = InsertCardType.HealthCard;
                //    return true;
                //}
                //else
                //{
                //    if (Config.IsUseTJCard)
                //    {
                //        if (CardNo.Length == Config.TJCardCount)
                //        {
                //            card = InsertCardType.HealthCard;
                //            return true;
                //        }

                //    }
                //}


                if (Config.LayoutName == LayoutEnum.AI || Config.LayoutName == LayoutEnum.JinYun)
                {
                #region  丽水社保长度37
                if (CardNo.Length >= 36)
                {
                    SunRise.HOSP.Common.Log.WriteModuleLogLn(SunRise.HOSP.Common.LogType.ErrorLog,
                        SunRise.HOSP.Common.LogModule.SocialSecurity, "社保卡:" + CardNo);
                    MoveCardToIC();
                    card = InsertCardType.SocialSecurityCard;
                    return true;
                }

              #region 丽水用
                if (CardNo.Length == 12)
                {
                    card = InsertCardType.HealthCard;
                    return true;
                }
               #endregion
                }

                card = InsertCardType.Unknown;

                return false; 
                #endregion 
            }



            if (ReadMagCardNo(ref CardNo) && CardNo.Length == 28)
            {
                card = InsertCardType.SocialSecurityCard;
                return true;
            }

            if (Config.IsLoopReadCard)
            {
                if (ReadMagCardNo(ref CardNo))
                {
                    if (CardNo.Length == Config.JZKCardCount)
                    {
                        card = InsertCardType.HealthCard;
                        return true;
                    }
                    else
                    {
                        if (Config.IsUseTJCard)
                        {
                            if (CardNo.Length == Config.TJCardCount)
                            {
                                card = InsertCardType.HealthCard;
                                return true;
                            }

                        }
                    }
                }



                if (ReadMagCardNo(ref CardNo) && CardNo.Length == 28)
                {
                    card = InsertCardType.SocialSecurityCard;
                    return true;
                }
            }

            if (m_SB != null && MoveCardToIC())//移到IC位读社保卡
            {
                card = InsertCardType.SocialSecurityCard;
                return true;
                #region    读卡 以为社保读卡,所以这里就直接返回
                //if (m_SB.ReadCardNo(ref CardNo))// && ci.CardIdentifier == "1" 
                //{
                //    if (String.IsNullOrWhiteSpace(CardNo))
                //        return false;
                //    card = InsertCardType.SocialSecurityCard;
                //    return true;
                //}
                //return false;
                #endregion
            }
            return false;
        }

        public bool ReadCardNoNC(ref InsertCardType card, ref string CardNo)
        {
            if (MoveCardToIC())//移到IC位读社保卡
            {
                string insuRegStr = string.Empty;
            

                return true;
            }
            return false;
        }

        /// <summary>
        /// 读取状态
        /// </summary>
        /// <param name="CardType"></param>
        /// <returns></returns>
        public bool GetCardType(ref InsertCardType card)
        {
            if (m_SB == null) return false;
            return m_SB.GetCardType(ref card);
        }

    }
}
