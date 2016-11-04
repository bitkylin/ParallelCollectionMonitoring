using System;
using System.Diagnostics;

namespace collectedReading.jsonBean.display
{
    public class CollectedResult
    {
        public CollectedResult(CollectItem collectItem)
        {
            序号 = collectItem.No;
            A = collectItem.A;
            B = collectItem.B;
            M = collectItem.M;
            电流值 = collectItem.Elec;
            var poles = collectItem.Poles;
            poles.ForEach(pole =>
            {
                switch (pole.Id)
                {
                    case 0:
                        Debug.WriteLine("电极0:"+ 电极0);
                        电极0 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 1:
                        电极1 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 2:
                        电极2 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 3:
                        电极3 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 4:
                        电极4 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 5:
                        电极5 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 6:
                        电极6 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 7:
                        电极7 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 8:
                        电极8 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 9:
                        电极9 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 10:
                        电极10 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 11:
                        电极11 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 12:
                        电极12 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 13:
                        电极13 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 14:
                        电极14 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 15:
                        电极15 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 16:
                        电极16 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 17:
                        电极17 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 18:
                        电极18 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 19:
                        电极19 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 20:
                        电极20 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 21:
                        电极21 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 22:
                        电极22 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 23:
                        电极23 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 24:
                        电极24 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 25:
                        电极25 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 26:
                        电极26 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 27:
                        电极27 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 28:
                        电极28 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 29:
                        电极29 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 30:
                        电极30 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 31:
                        电极31 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 32:
                        电极32 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 33:
                        电极33 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 34:
                        电极34 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 35:
                        电极35 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 36:
                        电极36 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 37:
                        电极37 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 38:
                        电极38 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 39:
                        电极39 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 40:
                        电极40 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 41:
                        电极41 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 42:
                        电极42 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 43:
                        电极43 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 44:
                        电极44 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 45:
                        电极45 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 46:
                        电极46 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 47:
                        电极47 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 48:
                        电极48 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 49:
                        电极49 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 50:
                        电极50 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 51:
                        电极51 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 52:
                        电极52 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 53:
                        电极53 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 54:
                        电极54 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 55:
                        电极55 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 56:
                        电极56 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 57:
                        电极57 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 58:
                        电极58 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 59:
                        电极59 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 60:
                        电极60 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 61:
                        电极61 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 62:
                        电极62 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                    case 63:
                        电极63 = Math.Round(pole.Value, PresetInfo.DecimalRound);
                        break;
                }
            });
        }

        public int 序号 { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int M { get; set; }
        public double 电流值 { get; set; }
        public double 电极0 { get; set; }
        public double 电极1 { get; set; }
        public double 电极2 { get; set; }
        public double 电极3 { get; set; }
        public double 电极4 { get; set; }
        public double 电极5 { get; set; }
        public double 电极6 { get; set; }
        public double 电极7 { get; set; }
        public double 电极8 { get; set; }
        public double 电极9 { get; set; }
        public double 电极10 { get; set; }
        public double 电极11 { get; set; }
        public double 电极12 { get; set; }
        public double 电极13 { get; set; }
        public double 电极14 { get; set; }
        public double 电极15 { get; set; }
        public double 电极16 { get; set; }
        public double 电极17 { get; set; }
        public double 电极18 { get; set; }
        public double 电极19 { get; set; }
        public double 电极20 { get; set; }
        public double 电极21 { get; set; }
        public double 电极22 { get; set; }
        public double 电极23 { get; set; }
        public double 电极24 { get; set; }
        public double 电极25 { get; set; }
        public double 电极26 { get; set; }
        public double 电极27 { get; set; }
        public double 电极28 { get; set; }
        public double 电极29 { get; set; }
        public double 电极30 { get; set; }
        public double 电极31 { get; set; }
        public double 电极32 { get; set; }
        public double 电极33 { get; set; }
        public double 电极34 { get; set; }
        public double 电极35 { get; set; }
        public double 电极36 { get; set; }
        public double 电极37 { get; set; }
        public double 电极38 { get; set; }
        public double 电极39 { get; set; }
        public double 电极40 { get; set; }
        public double 电极41 { get; set; }
        public double 电极42 { get; set; }
        public double 电极43 { get; set; }
        public double 电极44 { get; set; }
        public double 电极45 { get; set; }
        public double 电极46 { get; set; }
        public double 电极47 { get; set; }
        public double 电极48 { get; set; }
        public double 电极49 { get; set; }
        public double 电极50 { get; set; }
        public double 电极51 { get; set; }
        public double 电极52 { get; set; }
        public double 电极53 { get; set; }
        public double 电极54 { get; set; }
        public double 电极55 { get; set; }
        public double 电极56 { get; set; }
        public double 电极57 { get; set; }
        public double 电极58 { get; set; }
        public double 电极59 { get; set; }
        public double 电极60 { get; set; }
        public double 电极61 { get; set; }
        public double 电极62 { get; set; }
        public double 电极63 { get; set; }
    }
}