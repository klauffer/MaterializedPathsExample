using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaterializedPathsExample
{
    public class Service
    {
        private readonly Func<IEnumerable<FlatRecord>> _getFlatRecords;

        public Service(Func<IEnumerable<FlatRecord>> getFlatRecords)
        {
            _getFlatRecords = getFlatRecords;
        }

        public IEnumerable<Dto> GetDtos()
        {
            var records = _getFlatRecords();
            var dtos = InflateTree(records);
            return dtos;
        }

        private IEnumerable<Dto> InflateTree(IEnumerable<FlatRecord> records, int treeDepth = 1)
        {
            // Get all items at current level
            //var numberOfLayersInRoot = rootPath.Split('/').Length;
            var itemsAtCurrentLevel = records.Where(x => x.Path.Split('/').Length - 1 == treeDepth).ToList();
            // for each item at current level
            var dtos = new List<Dto>();
            foreach (var item in itemsAtCurrentLevel)
            {
                // Get all items below the current item
                var itemsBelowCurrentLevel = records.Where(x => x.Path.Split('/').Length - 1 > treeDepth).ToList();
                if (itemsBelowCurrentLevel.Any())
                {
                    var thisTreesDtos = InflateTree(itemsBelowCurrentLevel, ++treeDepth).ToList();
                    //*take returns, add them as children to root and return combined list
                    var dto = item.ProjectToDto();
                    dto.AddChildren(thisTreesDtos);
                    dtos.Add(dto);
                }
                else
                {
                    // if item doesnt have children then convert to dto and return
                    var dto = item.ProjectToDto();
                    dtos.Add(dto);
                }
            }
            return dtos;
        }
    }

    static class Projections
    {
        public static Dto ProjectToDto(this FlatRecord record) =>
            new Dto(record.Id);

        public static IEnumerable<Dto> ProjectToDto(this IEnumerable<FlatRecord> records) =>
            records.Select(ProjectToDto);
    }

    public class Dto
    {
        public int Id { get; }
        public List<Dto> Children { get; }

        public Dto(int id)
        {
            Id = id;
            Children = new List<Dto>();
        }

        public void AddChildren(IEnumerable<Dto> children) 
        {
            Children.AddRange(children);
        }
        

    }

    public class FlatRecord
    {
        public int Id { get; }
        public string Name { get; }
        public string Path { get;  }
        public FlatRecord(int id, string name, string path)
        {
            Id = id;
            Name = name;
            Path = path;
        }

    }
}