using Godot;
using System;

public class TopPrinter : Control
{
	private static TopPrinter topPrinter { get; set; }

	public static string One { get
		{
			if (topPrinter != null)
				return topPrinter._1.Text;
			else
				return "Not set!";
		}
		set
		{
			if (topPrinter != null)
				topPrinter._1.Text = value;
			else
				GD.Print("No top printer to set!");
		}
	}

	public static string Two
	{
		get
		{
			if (topPrinter != null)
				return topPrinter._2.Text;
			else
				return "Not set!";
		}
		set
		{
			if (topPrinter != null)
				topPrinter._2.Text = value;
			else
				GD.Print("No top printer to set!");
		}
	}

	public static string Three
	{
		get
		{
			if (topPrinter != null)
				return topPrinter._3.Text;
			else
				return "Not set!";
		}
		set
		{
			if (topPrinter != null)
				topPrinter._3.Text = value;
			else
				GD.Print("No top printer to set!");
		}
	}

	public static string Four
	{
		get
		{
			if (topPrinter != null)
				return topPrinter._1.Text;
			else
				return "Not set!";
		}
		set
		{
			if (topPrinter != null)
				topPrinter._4.Text = value;
			else
				GD.Print("No top printer to set!");
		}
	}


	Label _1;
	Label _2;
	Label _3;
	Label _4;

	public override void _Ready()
	{
		if (topPrinter == null)
			topPrinter = this;
		else
			QueueFree();

		_1 = (Label)GetNode("One");
		_2 = (Label)GetNode("Two");
		_3 = (Label)GetNode("Three");
		_4 = (Label)GetNode("Four");

		One = "";
		Two = "";
		Three = "";
		Four = "";
	}

}
