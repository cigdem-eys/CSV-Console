using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CSV_Console
{
    internal class Program
    {
        static System.Timers.Timer timerKontrolu = new System.Timers.Timer(1000);   // Saniyede bir kontrol et.
        static int saat = DateTime.Now.Hour;
        static void Main(string[] args)
        {
            timerKontrolu.Elapsed += new ElapsedEventHandler(SaatBasiniKontrolEt);

            timerKontrolu.Enabled = true;
            timerKontrolu.Start();
            Console.WriteLine("Saat: {0}. Uygulama başladı.Lütfen kapatmayınız.Saat başı olduğunda işlemler başlayacak...", DateTime.Now.ToString("%H:mm"));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("-------------------------------------------------------------------------");
            Console.ReadKey();


            Console.ReadKey();
        }
        static bool SayiMi(String strVeri)
        {
            if (String.IsNullOrEmpty(strVeri) == true)
            {
                return false;
            }
            else
            {
                Regex desen = new Regex("^[0-9]*$");
                return desen.IsMatch(strVeri);
            }
        }
        
        private static void SaatBasiniKontrolEt(object source, ElapsedEventArgs e)
        {
            

            if (saat < DateTime.Now.Hour || (saat == 23 && DateTime.Now.Hour == 0))
            {
               
                saat = DateTime.Now.Hour;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Saat: {0}", saat);
                Console.WriteLine("Saat başı işlem devrede...");
                SeriPortVeriOkuma();
            }
        }
        
        //Her saat başı çalış
        private static void SeriPortVeriOkuma()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Saat: {0}. Kuyruktaki veriler okunmaya başladı.", DateTime.Now.ToString("%H:mm"));
            string klasor = @"C:\Dosyalar";
            bool Kontrol = Directory.Exists(@"C:\Dosyalar");
            if (Kontrol)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Klasöre erişim sağlandı...");
                string[] files = Directory.GetFiles(klasor);
                if (files.Length > 0)
                {
                    Console.WriteLine(files.Length + " adet dosya bulundu...");

                    foreach (string f in files)
                    {
                        //Console.WriteLine("["+f + "] isimli dosya işleniyor...");

                        string dosya = f;
                        int dosyakayitsayisi = 0;

                        using (StreamReader sr = new StreamReader(dosya))
                        {
                            dosyakayitsayisi = sr.ReadLine().Length;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[" + f + "] isimli dosyada bulunan " + dosyakayitsayisi + " adet kayıt işleniyor...");

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("-------------------------------------------------------------------------");

                            //Header alanı lazım değil
                            sr.ReadLine().Remove(0, 1);

                            int kayitsayisi = 0;
                            int eklenen = 0;
                            int guncellenen = 0;
                            int ilk = 0;
                            int bos = 0;
                            DB_Context db = new DB_Context();
                            List<Product> product_list = db.Products.ToList();
                            List<Image> image_list = db.Images.ToList();
                            Product p = new Product();
                            Image img = new Image();
                            Console.ForegroundColor = ConsoleColor.Red;
                            while (!sr.EndOfStream)
                            {

                                string[] rows = sr.ReadLine().Split(',');

                                kayitsayisi++;
                                //satır işlemleri

                                p = new Product();
                                p.Barcode = rows[0] != null ? rows[0] : null;
                                p.Price = rows[1] != null ? Convert.ToDecimal(rows[1]) : 0;
                                p.Stock = rows[2] != null ? Convert.ToInt32(rows[2]) : 0;
                                p.Name = rows[3] != null ? rows[3] : null;

                                var product = product_list.Where(k => k.Barcode == p.Barcode).FirstOrDefault();
                                if (product != null)//Ürün mevcut
                                {
                                    //Console.WriteLine("[" + p.Name + "] isimli Ürün veritabanında kayıtlı, fiyat ve stok bilgileri kontrol ediliyor...");


                                    if (product.Price != p.Price && product.Stock != p.Stock)
                                    {
                                        //Console.WriteLine("[" + p.Name + "] isimli ürünün fiyat ve stok bilgileri güncelleniyor...");
                                        //Güncelleme bilgisi girecek 

                                        product.Price = p.Price;
                                        product.Stock = p.Stock;
                                        db.SaveChanges();
                                        guncellenen++;
                                    }

                                }
                                else
                                {
                                    //Kaydetme bilgisi girecek

                                    db.Products.Add(p);
                                    db.SaveChanges();
                                    //Console.WriteLine("[" + p.Name + "] isimli ürün eklendi.");
                                    eklenen++;
                                }
                            }


                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("-------------------------------------------------------------------------");
                            //Console.Clear();
                            Console.WriteLine(" ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(eklenen + " adet ürün veritabanına kayıt edildi");
                            Console.WriteLine(guncellenen + " adet ürün güncellendi");

                        }
                    }
                }
                else
                    Console.WriteLine("Okuma başarısız");
            }
            else
                Console.WriteLine("Klasör bulunamadı");
        }
    }
}
