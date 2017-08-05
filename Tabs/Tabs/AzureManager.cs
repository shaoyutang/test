using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tabs.DataModels;

public class AzureManager
{
     
    private static AzureManager instance;
    private MobileServiceClient client;
    private IMobileServiceTable<easytable> table;

    private AzureManager()
    {
        this.client = new MobileServiceClient("http://notcbd.azurewebsites.net");
        this.table = this.client.GetTable<easytable>();
    }

    public MobileServiceClient AzureClient
    {
        get { return client; }
    }

    public static AzureManager AzureManagerInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new AzureManager();
            }

            return instance;
        }
    }

    public async Task<List<easytable>> GetInformation()
    {
        return await this.table.ToListAsync();
    }


    public async Task PostInformation(easytable data)
    {
        await this.table.InsertAsync(data);
    }
}