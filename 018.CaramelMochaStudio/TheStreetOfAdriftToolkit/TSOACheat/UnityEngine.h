#pragma once

#include "Il2Cpp.h"
namespace UnityEngine
{
	/// <summary>
	/// 模块名
	/// </summary>
	constexpr const char* ModuleName = "UnityEngine.CoreModule";

	/// <summary>
	/// 对象类
	/// </summary>
	class Object
	{
	public:
		Object() = delete;
		Object(const Object&) = delete;
		Object(Object&&) = delete;
		Object& operator=(const Object&) = delete;
		Object& operator=(Object&&) = delete;
		~Object() = delete;

	private:
		static const char* NameSpace;
		static const char* ClassName;

	public:
		/// <summary>
		/// 获取对象名称
		/// <para>[C#]object.name</para>
		/// </summary>
		static Il2CppString* GetName(Il2CppObject* obj);
		/// <summary>
		/// 帧生成销毁对象
		/// <para>[C#]Object.Destroy(obj)</para>
		/// </summary>
		static void Destroy(Il2CppObject* obj);
		/// <summary>
		/// 立即销毁对象
		/// <para>[C#]Object.DestroyImmediate(obj)</para>
		/// </summary>
		static void DestroyImmediate(Il2CppObject* obj);
	};

	/// <summary>
	/// 游戏对象
	/// </summary>
	class GameObject
	{
	public:
		GameObject() = delete;
		GameObject(const GameObject&) = delete;
		GameObject(GameObject&&) = delete;
		GameObject& operator=(const GameObject&) = delete;
		GameObject& operator=(GameObject&&) = delete;
		~GameObject() = delete;

	private:
		static const char* NameSpace;
		static const char* ClassName;

	public:
		/// <summary>
		/// 获取类型
		/// <para>Il2Cpp类型</para>
		/// </summary>
		static const Il2CppType* GetNativeType();
		/// <summary>
		/// 获取组件
		/// </summary>
		/// <param name="gameObj">对象</param>
		/// <param name="type">组件类型</param>
		/// <param name="useSearchTypeAsArrayReturnType">是否使用查询的类型用作返回值</param>
		/// <param name="recursive">是否递归搜索</param>
		/// <param name="includeInactive">是否搜索非活动组件</param>
		/// <param name="reverse">是否反向搜索</param>
		/// <param name="resultList">List对象</param>
		static Il2CppArray* GetComponentsInternal(Il2CppObject* gameObj, const Il2CppObject* type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, void* resultList);
		/// <summary>
		/// 获取子组件
		/// </summary>
		/// <param name="gameObj">对象</param>
		/// <param name="type">组件类型</param>
		/// <param name="includeInactive">是否搜索非活动组件</param>
		static Il2CppArray* GetComponentsInChildren(Il2CppObject* gameObj, const Il2CppObject* type, bool includeInactive);
		/// <summary>
		/// 获取父组件
		/// </summary>
		/// <param name="gameObj">对象</param>
		/// <param name="type">组件类型</param>
		/// <param name="includeInactive">是否搜索非活动组件</param>
		static Il2CppArray* GetComponentsInParent(Il2CppObject* gameObj, const Il2CppObject* type, bool includeInactive);
	};

	/// <summary>
	/// 组件
	/// </summary>
	class Component
	{
	public:
		Component() = delete;
		Component(const Component&) = delete;
		Component(Component&&) = delete;
		Component& operator=(const Component&) = delete;
		Component& operator=(Component&&) = delete;
		~Component() = delete;

	private:
		static const char* NameSpace;
		static const char* ClassName;

	public:
		/// <summary>
		/// 新建对象
		/// <para>[C#]new Component()</para>
		/// </summary>
		static Il2CppObject* New();
		/// <summary>
		/// 获取类型
		/// <para>Il2Cpp类型</para>
		/// </summary>
		static const Il2CppType* GetNativeType();
		/// <summary>
		/// 获取游戏对象
		/// <para>[C#]component.gameObject</para>
		/// </summary>
		static Il2CppObject* GetGameObject(Il2CppObject* component);
		/// <summary>
		/// 获取游戏对象
		/// <para>[C#]component.transform</para>
		/// </summary>
		static Il2CppObject* GetTransform(Il2CppObject* component);
	};

	class Transform
	{
	public:
		Transform() = delete;
		Transform(const Transform&) = delete;
		Transform(Transform&&) = delete;
		Transform& operator=(const Transform&) = delete;
		Transform& operator=(Transform&&) = delete;
		~Transform() = delete;

	private:
		static const char* NameSpace;
		static const char* ClassName;

	public:
		/// <summary>
		/// 获取父对象
		/// <para>[C#]trans.parent</para>
		/// </summary>
		static Il2CppObject* GetParent(Il2CppObject* transform);
		/// <summary>
		/// 设置父对象
		/// <para>[C#]trans.parent = obj</para>
		/// </summary>
		static void SetParent(Il2CppObject* transform, Il2CppObject* parent);
	};
}