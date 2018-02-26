using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Meal
{
    public string name;//菜名
    public float cost;//价格（12.99＄ 2300两银 23￥）
    public int type;//类别（不可使用&运算）
    public int tag1;//类别标签（可以使用&运算）
    public int tag2;//菜品标签（可以使用&运算）
    public Dictionary<int, int> ingredients;//食材
    public float prograss;//研发进度 （大于1可以制作 之后精益求精）
    public string describe;//描述
    public GameObject model;//实体模型
    public Texture2D texture;//展示图片

    /******************************/
    public static Meal ShuiZhuRouPian
    {
        get
        {
            Meal temp = new Meal()
            {
                name = "水煮肉片",
                cost = 20,
                type = MealType.CHUAN,
                tag1 = MealTag1.HunCai,
                tag2 = MealTag2.MaLa & MealTag2.Xian,
                prograss = 1,
                describe = "红彤彤，香喷喷，来试试吧。",
                model = (GameObject)Resources.Load("ShuiZhuRouPian")
            };
            temp.ingredients = new Dictionary<int, int>();
            temp.ingredients.Add(Ingredient.ShaoRou, 2);
            temp.ingredients.Add(Ingredient.QingCai, 1);
            return temp;
        }
    }
    public static Meal SuSanXian
    {
        get
        {
            Meal temp = new Meal()
            {
                name = "素三鲜",
                cost = 12,
                type = MealType.JiaChangCai,
                tag1 = MealTag1.SuCai,
                tag2 = MealTag2.Xian,
                prograss = 1,
                describe = "焦香四溢，香鲜软糯，比肉还好吃哦。",
                model = (GameObject)Resources.Load("SuSanXian")
            };
            temp.ingredients = new Dictionary<int, int>();
            temp.ingredients.Add(Ingredient.QingCai, 3);
            return temp;
        }
    }

    private static Meal[] meals =
    {
        ShuiZhuRouPian,
        SuSanXian
    };

    public static Meal GetRandomMeal()
    {
        return meals[(int)(Random.value * meals.Length)];
    }

    public static Meal GetMeal(string name)
    {
        switch (name)
        {
            case "ShuiZhuRouPian": return ShuiZhuRouPian;
            case "SuSanXian": return SuSanXian;
        }
        return null;
    }

    /******************************/

    class MealTag1
    {
        /// <summary>
        /// 主食
        /// </summary>
        public static int ZhuShi = 0x0001;
        /// <summary>
        /// 荤菜
        /// </summary>
        public static int HunCai = 0x0002;
        /// <summary>
        /// 素菜
        /// </summary>
        public static int SuCai = 0x0004;
        /// <summary>
        /// 汤羹
        /// </summary>
        public static int TangGeng = 0x0008;
        /// <summary>
        /// 小食
        /// </summary>
        public static int XiaoShi = 0x0010;
        /// <summary>
        /// 酒
        /// </summary>
        public static int Jiu = 0x0020;

        /// <summary>
        /// 面食
        /// </summary>
        public static int MianShi = 0x0040;



    }

    class MealTag2
    {
        /// <summary>
        /// 酸
        /// </summary>
        public static int Suan = 0x0001;
        /// <summary>
        /// 甜
        /// </summary>
        public static int Tian = 0x0002;
        /// <summary>
        /// 苦
        /// </summary>
        public static int Ku = 0x0004;
        /// <summary>
        /// 咸
        /// </summary>
        public static int Xian = 0x0008;


        /// <summary>
        /// 麻
        /// </summary>
        public static int MaLa = 0x0010;


        /// <summary>
        /// 臭
        /// </summary>
        public static int Chou = 0x0100;
    }

    class MealType
    {
        /// <summary>
        /// 鲁菜
        /// </summary>
        public static int LU = 1;
        /// <summary>
        /// 川菜
        /// </summary>
        public static int CHUAN = 2;
        /// <summary>
        /// 粤菜
        /// </summary>
        public static int YUE = 3;
        /// <summary>
        /// 苏菜
        /// </summary>
        public static int SU = 4;
        /// <summary>
        /// 闽菜
        /// </summary>
        public static int MIN = 5;
        /// <summary>
        /// 浙菜
        /// </summary>
        public static int ZHE = 6;
        /// <summary>
        /// 湘菜
        /// </summary>
        public static int XIANG = 7;
        /// <summary>
        /// 徽菜
        /// </summary>
        public static int HUI = 8;

        /// <summary>
        /// 家常菜
        /// </summary>
        public static int JiaChangCai = 0;

        /// <summary>
        /// 西餐
        /// </summary>
        public static int Western_style = 9;
    }

    class Ingredient
    {
        string name;
        float cost;

        /**********************/

        /// <summary>
        /// 米类
        /// </summary>
        public static int Mi = 1;
        /// <summary>
        /// 面类
        /// </summary>
        public static int Mian = 2;
        /// <summary>
        /// 青菜
        /// </summary>
        public static int QingCai = 3;
        /// <summary>
        /// 山珍
        /// </summary>
        public static int ShanZhen = 4;
        /// <summary>
        /// 海味
        /// </summary>
        public static int HaiWei = 5;
        /// <summary>
        /// 鲜汤
        /// </summary>
        public static int XianTang = 6;
        /// <summary>
        /// 饺子
        /// </summary>
        public static int JiaoZi = 7;
        /// <summary>
        /// 烧肉
        /// </summary>
        public static int ShaoRou = 8;
        /// <summary>
        /// 西餐
        /// </summary>
        public static int XiCan = 9;
        /// <summary>
        /// 酒类
        /// </summary>
        public static int JiuLei = 10;
        /// <summary>
        /// 汤圆
        /// </summary>
        public static int TangYuan = 11;
    }

}




public class Order
{
    public Customer customer;
    public Transform MealPos;
    /*******Cook*******/
    public Meal[] meal;//具体的食物
    public int index;//食物的位置

    /*******Waiter*******/
    public bool available = true;//是否开始执行
    public string tag = "seat";

    public Transform trf;
    public callback fun;


    /*******类型*******/
    public delegate object callback(params object[] obj);
}

