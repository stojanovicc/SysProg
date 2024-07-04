using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Projekat3
{
    public class ArticleStream : IObservable<Article>
    {
        private readonly Subject<Article> subject = new Subject<Article>();
        private readonly ArticleService service = new ArticleService();
        public bool HasArticles { get; private set; }

        public async Task GetArticlesAsync(string keyword, string category)
        {
            try
            {
                var articles = await service.FetchArticles(keyword, category);

                HasArticles = articles != null && articles.Any();

                foreach (var article in articles)
                {
                    subject.OnNext(article);
                }
                subject.OnCompleted();
            }
            catch (Exception ex)
            {
                subject.OnError(ex);
            }
        }

        public IDisposable Subscribe(IObserver<Article> observer)
        {
            return subject
                .SubscribeOn(TaskPoolScheduler.Default)
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(observer);
        }
    }
}
