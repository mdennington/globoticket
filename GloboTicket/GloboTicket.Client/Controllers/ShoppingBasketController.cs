using System;
using System.Linq;
using System.Threading.Tasks;
using GloboTicket.Messaging;
using GloboTicket.Web.Extensions;
using GloboTicket.Web.Messages;
using GloboTicket.Web.Models;
using GloboTicket.Web.Models.Api;
using GloboTicket.Web.Models.View;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GloboTicket.Web.Controllers
{
    public class ShoppingBasketController : Controller
    {
        private readonly IShoppingBasketService basketService;
        private readonly Settings settings;
        private readonly IMessageBus _messageBus;

        public ShoppingBasketController(IShoppingBasketService basketService, Settings settings, IMessageBus messageBus)
        {
            this.basketService = basketService;
            this.settings = settings;
            _messageBus = messageBus;
        }

        public async Task<IActionResult> Index()
        {
            var basketLines = await basketService.GetLinesForBasket(Request.Cookies.GetCurrentBasketId(settings));
            var lineViewModels = basketLines.Select(bl => new BasketLineViewModel
            {
                LineId = bl.BasketLineId,
                EventId = bl.EventId,
                EventName = bl.Event.Name,
                Date = bl.Event.Date,
                Price = bl.Price,
                Quantity = bl.TicketAmount
            }
            );
            return View(lineViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLine(BasketLineForCreation basketLine)
        {
            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            var newLine = await basketService.AddToBasket(basketId, basketLine);
            Response.Cookies.Append(settings.BasketIdCookieName, newLine.BasketId.ToString());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLine(BasketLineForUpdate basketLineUpdate)
        {
            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            await basketService.UpdateLine(basketId, basketLineUpdate);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveLine(Guid lineId)
        {
            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            await basketService.RemoveLine(basketId, lineId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Pay()
        {
            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            await _messageBus.PublishMessage(new PaymentMessage{ BasketId = basketId }, "payments");
            return View("Thanks");
        }
    }
}
