﻿using FluentAssertions;
using NUnit.Framework;

namespace UniqueDb.ConnectionProvider.Tests.DataGeneration.SqlManipulation.PiecewiseManipulation;

[TestFixture]
public class TreeFlattenerTests
{
   [Test]
   public async Task Test1()
   {
      var node    = new Node("root");
      var results = Flattener.Flatten(node, x => x.Children);
      results.Should().HaveCount(1);
      results[0].Should().HaveCount(1);
      results[0].Should().Equal(new List<Node>(){node});
   }

   [Test]
   public async Task Test1Child()
   {
      var node = new Node("root");
      var child1 = new Node("child1");
      node.Children.Add(child1);
      var results = Flattener.Flatten(node, x => x.Children);
      results.Should().HaveCount(2);
      results[0].Should().HaveCount(1);
      results[0].Should().Equal(new List<Node>(){node});
      results[1].Should().HaveCount(2);
      results[1].Should().Equal(new List<Node>(){node, child1});
   }

   [Test]
   public async Task Test2DeepChild()
   {
      var node = new Node("root");
      var child1 = new Node("child1");
      node.Children.Add(child1);
      var child2 = new Node("child1.child1");
      child1.Children.Add(child2);

      var results = Flattener.Flatten(node, x => x.Children);
      results.Should().HaveCount(3);
      results[0].Should().HaveCount(1);
      results[0].Should().Equal(new List<Node>(){node});
      results[1].Should().HaveCount(2);
      results[1].Should().Equal(new List<Node>(){node, child1});
      results[2].Should().HaveCount(3);
   }
   
   [Test]
   public async Task Test2WideChild()
   {
      var node = new Node("root");
      var child1 = new Node("child1");
      node.Children.Add(child1);
      var child2 = new Node("child1.child1");
      node.Children.Add(child2);

      var results = Flattener.Flatten(node, x => x.Children);
      results.Should().HaveCount(3);
      results[0].Should().HaveCount(1);
      results[0].Should().Equal(new List<Node>(){node});
      results[1].Should().HaveCount(2);
      results[1].Should().Equal(new List<Node>(){node, child1});
      results[2].Should().HaveCount(2);
      results[2].Should().Equal(new List<Node>(){node, child2});
   }
}

public static class Flattener
{
   /// <summary>
   /// Returns a list containing many lists. Each list represents a path from the root item to a (grand)child.
   /// </summary>
   /// <param name="item"></param>
   /// <param name="childFunc"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   public static List<List<T>> Flatten<T>(T item, Func<T, List<T>> childFunc)
   {
      var seen  = new HashSet<T>();
      var queue = new Queue<List<T>>();
      queue.Enqueue(new List<T>(){item});
      var output = new List<List<T>>();

      while (queue.Count > 0)
      {
         var list = queue.Dequeue();
         output.Add(list);
         foreach (var children in childFunc(list.Last()))
         {
            var newlist = new List<T>();
            newlist.AddRange(list);
            newlist.Add(children);
            queue.Enqueue(newlist);
         }
      }

      return output;
   }
}

public class Node
{
   public string          Name     { get; set; }
   public List<Node> Children { get; set; } = new();

   public Node()
   {
      
   }

   public Node(string name)
   {
      Name = name;
   }
}

