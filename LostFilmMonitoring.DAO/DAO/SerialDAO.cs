using LostFilmMonitoring.DAO.DomainModels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LostFilmMonitoring.DAO.DAO
{
    public class SerialDAO : BaseDAO
    {
        public SerialDAO(string connectionString) : base(connectionString)
        {
        }

        public async Task<Serial> LoadAsync(string name)
        {
            using (var ctx = OpenContext())
            {
                return await ctx.Serials.FirstOrDefaultAsync(s => s.Name == name);
            }
        }

        public async Task<List<Serial>> LoadAsync()
        {
            using (var ctx = OpenContext())
            {
                return await ctx.Serials.ToListAsync();
            }
        }

        public async Task SaveAsync(Serial serial)
        {
            using (var ctx = OpenContext())
            {
                var existingSerial = await ctx.Serials.FirstOrDefaultAsync(s => s.Name == serial.Name);
                if (existingSerial == null)
                {
                    ctx.Add(serial);
                }
                else
                {
                    existingSerial.LastEpisode = serial.LastEpisode;
                    existingSerial.LastEpisodeName = serial.LastEpisodeName;
                    existingSerial.LastEpisodeTorrentLinkSD = serial.LastEpisodeTorrentLinkSD;
                    existingSerial.LastEpisodeTorrentLinkMP4 = serial.LastEpisodeTorrentLinkMP4;
                    existingSerial.LastEpisodeTorrentLink1080 = serial.LastEpisodeTorrentLink1080;
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}