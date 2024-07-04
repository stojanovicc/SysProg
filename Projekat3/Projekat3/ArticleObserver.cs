using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class ArticleObserver : IObserver<Article>
    {
        private readonly string name;

        public ArticleObserver(string name)
        {
            this.name = name;
        }

        public void OnNext(Article article)
        {
            Console.WriteLine($"Title: {article.Title}\n" +
                              $"Source: {article.Source}\n" +
                              $"Topics: \n{ string.Join(", \n", article.Topics)}");
        }

        public void OnError(Exception e)
        {
            Console.WriteLine($"Greska: {e.Message}\n");
        }

        public void OnCompleted()
        {
            Console.WriteLine("Svi clanci su obradjeni!");
        }
    }
}
