using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Trash.Pma
{
    using Exceptions;

    /// <summary>
    /// プライベートメソッドを呼び出すアクセッサ―を提供します。
    /// </summary>
    public class MethodAccessor : DynamicObject
    {
        /// <summary>存在確認を行うか設定</summary>
        private bool confirmDefined = true;

        /// <summary>検索条件を設定</summary>
        private static BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        /// <summary>アクセッサ―を利用するインスタンス</summary>
        public readonly object Instance;

        /// <summary>アクセッサ―を利用する型</summary>
        public readonly Type Type;

        /// <summary>可変長のパラメーターを受け取って TResult パラメーターに指定された型の値を返すメソッドをカプセル化します。</summary>
        public delegate TResult Func<in @params, out TResult>(params @params[] parameters);

        /// <summary>
        /// アクセッサ―のインスタンスを初期化します。
        /// </summary>
        /// <param name="instance">利用するインスタンス</param>
        public MethodAccessor(object instance) : this(instance, flags) { }

        /// <summary>
        /// アクセッサ―のインスタンスを初期化します。
        /// </summary>
        /// <param name="instance">利用するインスタンス</param>
        /// <param name="bindFlags">対象メソッドの条件</param>
        public MethodAccessor(object instance, BindingFlags bindFlags)
        {
            Instance = instance;
            Type = Instance.GetType();
            flags = bindFlags;
        }

        /// <summary>
        /// 指定した名前のメソッドを取得します。
        /// </summary>
        public dynamic this[string name]
        {
            get { return CreateMethod(name); }
        }

        /// <summary>
        /// 指定されたインスタンスにメソッドが存在するか検証します。
        /// </summary>
        /// <param name="name">対象のメソッド名</param>
        /// <param name="types">オーバーロード</param>
        /// <returns>定義されているか</returns>
        public bool IsDefined(string name, params Type[] types) => null != GetMethod(name, types);
        
        /// <summary>
        /// 指定されたインスタンスに存在するメソッドを取得します。
        /// </summary>
        /// <param name="name">対象のメソッド名</param>
        /// <param name="types">オーバーロード</param>
        /// <returns>条件に一致したメソッド</returns>
        public MethodInfo GetMethod(string name, params Type[] types) => Type.GetMethod(name, flags, null, types, null);

        /// <summary>
        /// 指定された引数一覧の型を返します。
        /// </summary>
        /// <param name="args">引数一覧</param>
        /// <returns>型一覧</returns>
        public Type[] GetArgsType(params object[] args) => args.Select(type => type.GetType()).ToArray();

        /// <summary>
        /// メンバー値を設定する演算の実装を提供します。System.Dynamic.DynamicObject クラスの派生クラスでこのメソッドをオーバーライドして、プロパティ値の設定などの演算の動的な動作を指定できます。
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = CreateMethod(binder.Name);
            return true;
        }

        /// <summary>
        /// メソッドを作成します
        /// </summary>
        private dynamic CreateMethod(string name)
        {
            return new Func<object, object>(args =>
            {
                var types = GetArgsType(args);
                var info = GetMethod(name, types);

                if (info == null)
                {
                    if (confirmDefined) return null;

                    throw new MethodDoesNotExistException(name);
                }
                return info.Invoke(Instance, args);
            });
        }

        /// <summary>
        /// メソッドの存在を確認するか設定する
        /// </summary>
        public bool ConfirmDefined
        {
            get { return confirmDefined; }
            set { confirmDefined = value; }
        }
    }
}