using System;

namespace SelfObjectModel
{

    /*public class Message
    {

    }
    public class PrintMessage : Message
    {
        public SelfObject obj;
        public PrintMessage(SelfObject obj)
        {
            this.obj = obj;
        }
    }
    public class EvaluateMessage : Message
    {
        public SelfObject obj;
        public EvaluateMessage(SelfObject obj)
        {
            this.obj = obj;
        }
    }
    public class CopyMessage : Message
    {
        public SelfObject obj;
        public CopyMessage(SelfObject obj)
        {
            this.obj = obj;
        }
    }
    public class SendAMessageMessage : Message
    {
        public SelfObject receiver;
        public string msg;
        public SendAMessageMessage(SelfObject receiver, string msg)
        {
            this.receiver = receiver;
            this.msg = msg;
        }
    }
    public class SendAMessageWithParametersMessage : Message
    {
        public SelfObject receiver;
        public string msg;
        public SelfObject parameter;
        public SendAMessageWithParametersMessage(SelfObject receiver, string msg, SelfObject parameter)
        {
            this.receiver = receiver;
            this.msg = msg;
            this.parameter = parameter;
        }
    }
    public class AssignSlotMessage : Message
    {
        public SelfObject obj;
        public string name;
        public SelfObject slot;
        public AssignSlotMessage(SelfObject obj, string name, SelfObject slot)
        {
            this.obj = obj;
            this.name = name;
            this.slot = slot;
        }
    }
    public class MakeParentMessage : Message
    {
        public SelfObject obj;
        public string name;
        public MakeParentMessage(SelfObject obj, string name)
        {
            this.obj = obj;
            this.name = name;
        }
    }
    public class AssignParentSlotMessage : Message
    {
        public SelfObject obj;
        public string name;
        public SelfObject parent;
        public AssignParentSlotMessage(SelfObject obj, string name, SelfObject parent)
        {
            this.obj = obj;
            this.name = name;
            this.parent = parent;
        }
    }*/
    public class SelfObject
    {
        private static int idCounter = 0;
        public int id;
        // value held by selfobject
        public object? val;
        public Func<SelfObject, SelfObject>? primitiveFunction;
        // slots held in slots dictionary
        public Dictionary<string, SelfObject?> slots = new Dictionary<string, SelfObject?>();
        // parent slots held in parents
        public Dictionary<string, SelfObject?> parents = new Dictionary<string, SelfObject?>();
        //arguments
        public SelfObject? parameter;
        // optional messages to be sent when evaluated
        public string[]? messages;

        public SelfObject? result;//mediator for storing values
        public SelfObject()
        {
            id = idCounter++;
        }

        //creates a selfobject with the given value and messages, and no slots or parents
        public SelfObject(object val)
        {
            this.val = val;
            id = idCounter++;
        }
        // creates a selfobject with selfobject
        private SelfObject(SelfObject obj)
        {
            this.val = obj.val;
            this.slots = obj.slots;
            this.parents = obj.parents ?? new Dictionary<string, SelfObject?>();
            this.messages = obj.messages;
            this.primitiveFunction = obj.primitiveFunction;
            this.parameter = obj.parameter;
            id = idCounter++;
        }

        // prints the selfobject's value, slots, and parents
        public static void Print(SelfObject obj)
        {
            System.Console.WriteLine($"SelfObject(Val: {obj.val})");
            if (obj.parents.Count > 0)
            {
                System.Console.WriteLine(" Parents:");
                foreach (var parent in obj.parents)
                {
                    System.Console.WriteLine($"  {parent.Key}: {parent.Value}");
                }
            }
            if (obj.slots.Count > 0)
            {
                System.Console.WriteLine(" Slots:");
                foreach (var slot in obj.slots)
                {
                    System.Console.WriteLine($"  {slot.Key}: {slot.Value}");
                }
            }
            if (obj.messages != null && obj.messages.Length > 0)
            {
                System.Console.WriteLine(" Messages:");
                foreach (string msg in obj.messages)
                {
                    System.Console.WriteLine($"  {msg}");
                }
            }
        }
        // evaluates selfobject by either returning itself or sending its messages to a copy of itself
        public static SelfObject evaluate(SelfObject obj)
        {
            if (obj.primitiveFunction != null)
            {
                SelfObject obj2 = Copy(obj);
                return obj2.primitiveFunction!(obj2);
            }
            if (obj.messages != null && obj.messages.Length > 0)
            {
                SelfObject obj2 = Copy(obj);
                SelfObject result = obj2;
                for (int i = 0; i < obj2.messages!.Length; i++)
                {
                    SendAMessage(obj2, obj2.messages[i]);
                }
                return result;
            }
            return obj;
        }
        // creates a copy of the selfobject as a slot named "copy"
        public static SelfObject Copy(SelfObject obj)
        {
            SelfObject copy = new SelfObject(obj);
            AssignParentSlot(copy, "copyof", obj);
            return copy;
        }
        public static SelfObject findObject(SelfObject receiver, string msg)
        {
            int V = idCounter;
            bool[] visited = new bool[V];

            SelfObject src = receiver;
            Queue<SelfObject> q = new Queue<SelfObject>();
            visited[src.id] = true;
            q.Enqueue(src);

            while (q.Count > 0)
            {
                SelfObject curr = q.Dequeue();
                foreach (string slot in curr.slots.Keys)
                {
                    if (slot.Equals(msg))
                    {
                        return curr.slots[slot]!;
                    }
                }
                // visit all the unvisited
                // neighbours of current node
                foreach (SelfObject? x in curr.parents.Values)
                {
                    if (x != null && !visited[x.id])
                    {
                        visited[x.id] = true;
                        q.Enqueue(x);
                    }
                }
            }

            throw new Exception("Message not found");
        }
        // sends a message to a receiver selfobject with some message representing a slot name
        //check slots and then recursive message parents
        public static SelfObject SendAMessage(SelfObject receiver, string msg)
        {
            return evaluate(findObject(receiver, msg));
        }
        public static SelfObject SendAMessageWithParameters(SelfObject receiver, string msg, SelfObject parameter)
        {
            SelfObject obj = findObject(receiver, msg);
            obj.parameter = parameter;
            return evaluate(obj);
        }
        public static void AssignSlot(SelfObject obj, string name, SelfObject slot)
        {
            if (obj.slots.ContainsKey(name))
            {
                obj.slots[name] = slot;
                return;
            }
            obj.slots.Add(name, slot);
        }
        public static void MakeParent(SelfObject obj, string name)
        {
            if (obj.parents.ContainsKey(name))
            {
                obj.parents[name] = obj.slots[name];
            }
            else
                obj.parents.Add(name, obj.slots[name]);
            obj.slots.Remove(name);
        }
        public static void AssignParentSlot(SelfObject obj, string name, SelfObject parent)
        {
            AssignSlot(obj, name, parent);
            MakeParent(obj, name);
        }
    }
    public class Program : SelfObject
    {
        public Program(object val) : base(val) { }
        public static void Main(string[] args)
        {
            SelfObject five = new SelfObject(5);
            SelfObject ten = new SelfObject(10);
            SelfObject add = new SelfObject();
            add.primitiveFunction = (self) =>
            {
                Console.WriteLine($"Adding {self.val} and {self.parameter?.val} to get");
                if (self.val != null && self.parameter != null && self.parameter.val != null)
                {
                    int sum = (int)self.val + (int)self.parameter.val;
                    return new SelfObject(sum);
                }
                if (self.val == null && self.parameter != null && self.parameter.val != null)
                {
                    self.val = self.parameter.val;
                }
                return self;
            };
            SelfObject primitives = new SelfObject();
            SelfObject evaluation = new SelfObject();
            evaluation.primitiveFunction = (self) =>
            {
                return evaluate(self.parameter!);
            };
            SelfObject print = new SelfObject();
            print.primitiveFunction = (self) =>
            {
                Print(self.parameter!);
                return self.parameter!;
            };
            AssignSlot(primitives, "add", add);
            AssignSlot(primitives, "evaluate", evaluation);
            AssignSlot(primitives, "print", print);
            SendAMessageWithParameters(primitives, "print", five); // prints 5
            SelfObject addFive = SendAMessageWithParameters(primitives, "add", five);
            AssignSlot(primitives, "addFive", addFive);

            Console.WriteLine(SendAMessageWithParameters(primitives, "addFive", ten).val); // 15

            SelfObject TrueObj = new SelfObject(true);
            SelfObject FalseObj = new SelfObject(false);
            AssignSlot(TrueObj, "ifTrueifFalse", new SelfObject());
            AssignSlot(FalseObj, "ifTrueifFalse", new SelfObject());
            TrueObj.slots["ifTrueifFalse"]!.primitiveFunction = (self) =>
            {
                SelfObject? trueBranch = self.parameter?.slots["true"];
                if (trueBranch != null)
                {
                    return evaluate(trueBranch);
                }
                return self;
            };
            FalseObj.slots["ifTrueifFalse"]!.primitiveFunction = (self) =>
            {
                SelfObject? falseBranch = self.parameter?.slots["false"];
                if (falseBranch != null)
                {
                    return evaluate(falseBranch);
                }
                return self;
            };
            var param = new SelfObject();
            AssignSlot(param, "true", new SelfObject());
            AssignSlot(param, "false", new SelfObject());
            SendAMessageWithParameters(TrueObj, "ifTrueifFalse", param); // evaluates true branch
            SendAMessageWithParameters(FalseObj, "ifTrueifFalse", param); // evaluates false branch
        }
    }
}