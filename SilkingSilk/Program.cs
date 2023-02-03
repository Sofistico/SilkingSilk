using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace SilkingSilk
{
    internal class Program
    {
        private static IWindow _window;
        private static GL Gl;


        //Our new abstracted objects, here we specify what the types are.
        private static BufferObject<float> Vbo;
        private static BufferObject<uint> Ebo;
        private static VertexArrayObject<float, uint> Vao;
        private static SilkingSilk.Shader Shader;
        private static readonly float[] Vertices =
        {
            //X    Y      Z     R  G  B  A
             0.5f,  0.5f, 0.0f, 1, 0, 0, 1,
             0.5f, -0.5f, 0.0f, 0, 0, 0, 1,
            -0.5f, -0.5f, 0.0f, 0, 0, 1, 1,
            -0.5f,  0.5f, 0.5f, 0, 0, 0, 1
        };

        private static readonly uint[] Indices =
{
            0, 1, 3,
            1, 2, 3
        };

        private static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "My first Silk.NET application!"
            };

            _window = Silk.NET.Windowing.Window.Create(options);

            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClose;

            _window.Run();
        }

        private static unsafe void OnLoad()
        {
            IInputContext input = _window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
                input.Keyboards[i].KeyDown += KeyDown;

            Gl = GL.GetApi(_window);
            //Instantiating our new abstractions
            Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

            //Telling the VAO object how to lay out the attribute pointers
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 7, 0);
            Vao.VertexAttributePointer(1, 4, VertexAttribPointerType.Float, 7, 3);

            Shader = new SilkingSilk.Shader(Gl, "shader.vert", "shader.frag");
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
                _window.Close();
        }

        private static void OnUpdate(double dt)
        { }

        private static unsafe void OnRender(double dt)
        {
            Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            //Binding and using our VAO and shader.
            Vao.Bind();
            Shader.Use();
            //Setting a uniform.
            Shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));

            Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);

        }

        private static void OnClose()
        {
            //Remember to dispose all the instances.
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
            Shader.Dispose();
        }
    }
}