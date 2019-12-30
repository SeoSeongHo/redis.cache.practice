﻿using redis.cache.practice.models;
using redis.cache.practice.services.crawler;
using redis.cache.practice.services.redis;
using System.Linq;
using System.Threading.Tasks;

namespace redis.cache.practice.services.menuStore
{
    public class MenuStore : IMenuStore
    {
        private ITriggerCrawler triggerCrawler;

        public MenuStore(ITriggerCrawler triggerCrawler)
        {
            this.triggerCrawler = triggerCrawler;
        }

        public async Task<FoodMenu> GetMenu(string restaurant)
        {
            var menus = await RedisCache.Init.HashGetAll(restaurant);

            if (menus == null)
            {
                await triggerCrawler.CrawlHtmlAsync(CrawlerType.menu);
                menus = await RedisCache.Init.HashGetAll(restaurant);
            }

            return new FoodMenu
            {
                restaurant = restaurant,
                food = new Food
                {
                    breakfast = menus.FirstOrDefault(x => x.Name == "breakfast").ToString(),
                    lunch = menus.FirstOrDefault(x => x.Name == "lunch").ToString(),
                    dinner = menus.FirstOrDefault(x => x.Name == "dinner").ToString()
                }
            };
        }
    }
}
