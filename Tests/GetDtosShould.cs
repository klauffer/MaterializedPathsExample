using MaterializedPathsExample;

namespace Tests
{
    public class GetDtosShould
    {
        [Fact]
        public void FindASingleRecord()
        {
            var id = 1;
            var flatRecords = new List<FlatRecord>()
            {
                new FlatRecord(id, "Chapter 1", "/1")
            };

            var service = new Service(() => flatRecords);
            var dtos = service.GetDtos();

            Assert.Contains(dtos, x => x.Id == id);
        }

        [Fact]
        public void NotCareAboutContentsOfPath()
        {
            var flatRecords = new List<FlatRecord>()
            {
                new FlatRecord(1, "Chapter 1", "/ThermalThrockle"),
                new FlatRecord(2, "Chapter 1.1", "/ThermalThrockle/Replacement")
            };

            var service = new Service(() => flatRecords);
            var dtos = service.GetDtos();

            var parent = dtos.Single(x => x.Id == 1);
            var child = parent.Children.Single(x => x.Id == 2);
            Assert.Equal(2, child.Id);
        }

        [Fact]
        public void FindMultipleRoots()
        {
            var flatRecords = new List<FlatRecord>()
            {
                new FlatRecord(1, "Chapter 1", "/1"),
                new FlatRecord(2, "Chapter 2", "/2")
            };

            var service = new Service(() => flatRecords);
            var dtos = service.GetDtos();

            var parent1 = dtos.Single(x => x.Id == 1);
            Assert.Equal(1, parent1.Id);

            var parent2 = dtos.Single(x => x.Id == 2);
            Assert.Equal(2, parent2.Id);
        }

        [Fact]
        public void FindADescendant()
        {
            var databaseRecords = new List<FlatRecord>()
            {
                new FlatRecord(1, "Chapter 1", "/1"),
                new FlatRecord(2, "Section 1", "/1/2")
            };

            var service = new Service(() => databaseRecords);
            var dtos = service.GetDtos();
            var parent = dtos.Single(x => x.Id == 1);
            var child = parent.Children.Single(x => x.Id == 2);
            Assert.Equal(2, child.Id);
        }

        [Fact]
        public void FindMultipleDescendants()
        {
            var databaseRecords = new List<FlatRecord>()
            {
                new FlatRecord(1, "Chapter 1", "/1"),
                new FlatRecord(2, "Section 1", "/1/2"),
                new FlatRecord(3, "Section 2", "/1/3")
            };

            var service = new Service(() => databaseRecords);
            var dtos = service.GetDtos();
            var parent = dtos.Single(x => x.Id == 1);

            var child1 = parent.Children.First(x => x.Id == 2);
            Assert.Equal(2, child1.Id);

            var child2 = parent.Children.First(x => x.Id == 3);
            Assert.Equal(3, child2.Id);
        }

        [Fact]
        public void FindDistantDescendants()
        {
            var databaseRecords = new List<FlatRecord>()
            {
                new FlatRecord(1, "Chapter 1", "/1"),
                new FlatRecord(2, "Section 1", "/1/2"),
                new FlatRecord(3, "Section 1.1", "/1/2/3")
            };

            var service = new Service(() => databaseRecords);
            var dtos = service.GetDtos();
            var parent = dtos.Single(x => x.Id == 1);
            var firstChild = parent.Children.Single(x => x.Id == 2);
            Assert.Equal(2, firstChild.Id);

            var grandChild = firstChild.Children.Single(x => x.Id == 3);
            Assert.Equal(3, grandChild.Id);
        }

        [Fact]
        public void FindMultipleDistantDescendants()
        {
            var databaseRecords = new List<FlatRecord>()
            {
                new FlatRecord(1, "Chapter 1", "/1"),
                new FlatRecord(2, "Section 1", "/1/2"),
                new FlatRecord(3, "Section 1.1", "/1/2/3"),
                new FlatRecord(4, "Section 1.2", "/1/2/4")
            };

            var service = new Service(() => databaseRecords);
            var dtos = service.GetDtos();
            var parent = dtos.Single(x => x.Id == 1);
            var firstChild = parent.Children.Single(x => x.Id == 2);
            Assert.Equal(2, firstChild.Id);

            var grandChild1 = firstChild.Children.Single(x => x.Id == 3);
            Assert.Equal(3, grandChild1.Id);

            var grandChild2 = firstChild.Children.Single(x => x.Id == 4);
            Assert.Equal(4, grandChild2.Id);
        }
    }
}