﻿using PocDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WiremockUI.Data
{
    internal class Schema
    {
        public List<Proxy> Proxies { get; set; }
        public List<Settings> Settings { get; set; }
    }

    internal class UnitOfWork
    {
        public static PocFile<Schema> _staticPocFile;
        private readonly Dictionary<Type, object> repositories;

        public static PocFile<Schema> PocFile
        {
            get
            {
                if (_staticPocFile == null)
                    _staticPocFile = new PocFile<Schema>();
                return _staticPocFile;
            }
            set
            {
                _staticPocFile = value;
            }
        }

        public UnitOfWork()
        {
            this.repositories = new Dictionary<Type, object>();
        }
        
        public Repository<Proxy> Proxies
        {
            get
            {
                Repository<Proxy> repository;
                if (repositories.ContainsKey(typeof(Proxy)))
                    repository = (Repository<Proxy>)repositories[typeof(Proxy)];
                else
                    repository = new Repository<Proxy>(PocFile);

                return repository;
            }
        }

        public Repository<Settings> Settings
        {
            get
            {
                Repository<Settings> repository;
                if (repositories.ContainsKey(typeof(Settings)))
                    repository = (Repository<Settings>)repositories[typeof(Settings)];
                else
                    repository = new Repository<Settings>(PocFile);

                return repository;
            }
        }

        public void Save()
        {
            lock (_staticPocFile)
                PocFile.Save();
        }
    }

    internal class Repository<T>
    {
        private PocFile<Schema> pocFile;
        private PocRepository<Schema, T> repository;
        
        public Repository(PocFile<Schema> pocFile)
        {
            this.pocFile = pocFile;
            this.repository = this.pocFile.GetRepository<T>();
        }

        public IQueryable<T> AsQueryable()
        {
            return this.repository.AsQueryable();
        }

        public void Delete(Guid id)
        {
            this.repository.Delete(id);
        }

        public void Delete(T entityToDelete)
        {
            this.repository.Delete(entityToDelete);
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            return this.repository.Get(filter, orderBy, includeProperties);
        }

        public IEnumerable<T> GetAll()
        {
            return this.repository.GetAll();
        }

        public T GetById(Guid id)
        {
            return this.repository.GetById(id);
        }

        public void Insert(T entity)
        {
            this.repository.Insert(entity);
        }

        public void Update(T entityToUpdate)
        {
            this.repository.Update(entityToUpdate);
        }
    }
}
