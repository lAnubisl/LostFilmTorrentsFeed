using LostFilmMonitoring.DAO.DomainModels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
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
            try
            {
                using (var ctx = OpenContext())
                {
                    return await ctx.Serials.FirstOrDefaultAsync(s => s.Name == name);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Serial>> LoadAsync()
        {
            try
            {
                using (var ctx = OpenContext())
                {
                    return await ctx.Serials.ToListAsync();
                }
            }
            catch (Exception)
            {
                return null;
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
                    existingSerial.LastEpisodeLink = serial.LastEpisodeLink;
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}