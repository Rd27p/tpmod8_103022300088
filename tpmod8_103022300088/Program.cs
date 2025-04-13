using System;
using System.IO;
using System.Text.Json;

class CovidConfig
{
    public string SatuanSuhu { get; set; }
    public int BatasHariDeman { get; set; }
    public string PesanDitolak { get; set; }
    public string PesanDiterima { get; set; }

    private string configFile = "covid_config.json";

    public void LoadConfig()
    {
        if (File.Exists(configFile))
        {
            try
            {
                var configData = File.ReadAllText(configFile);
                var config = JsonSerializer.Deserialize<CovidConfig>(configData);
                SatuanSuhu = config.SatuanSuhu;
                BatasHariDeman = config.BatasHariDeman;
                PesanDitolak = config.PesanDitolak;
                PesanDiterima = config.PesanDiterima;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }
        }
        else
        {
            SaveConfig();
        }
    }
    public void SaveConfig()
    {
        try
        {
            var configData = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFile, configData);
            Console.WriteLine("Config file saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config: {ex.Message}");
        }
    }

    public void UbahSatuan()
    {
        if (SatuanSuhu == "celcius")
        {
            SatuanSuhu = "fahrenheit";
        }
        else
        {
            SatuanSuhu = "celcius";
        }
        SaveConfig();  
    }

    public string GetMessage(string status)
    {
        return status == "ditolak" ? PesanDitolak : PesanDiterima;
    }
}

class Program
{
    static void Main()
    {
        var config = new CovidConfig();
        config.LoadConfig();

        Console.WriteLine("Apakah Anda ingin mengubah satuan suhu? (y/n): ");
        string ubahSatuan = Console.ReadLine();
        if (ubahSatuan == "y")
        {
            config.UbahSatuan();
            Console.WriteLine($"Satuan suhu telah diubah menjadi {config.SatuanSuhu}.");
        }

        Console.WriteLine($"Satuan suhu saat ini adalah {config.SatuanSuhu}.");
        Console.WriteLine(" ");
        Console.WriteLine($"Berapa suhu badan anda saat ini? ({config.SatuanSuhu}) ");
        double suhu = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine(" ");
        Console.WriteLine("Berapa hari yang lalu anda terakhir memiliki gejala demam?");
        int hariDeman = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine(" ");
        if (config.SatuanSuhu == "celcius")
        {
            if (suhu < 36.5 || suhu > 37.5)
            {
                Console.WriteLine(config.GetMessage("ditolak"));
                return;
            }
        }
        else 
        {
            if (suhu < 97.7 || suhu > 99.5)
            {
                Console.WriteLine(config.GetMessage("ditolak"));
                return;
            }
        }

        if (hariDeman >= config.BatasHariDeman)
        {
            Console.WriteLine(config.GetMessage("ditolak"));
            return;
        }

        Console.WriteLine(config.GetMessage("diterima"));
    }
}