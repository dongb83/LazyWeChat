using LazyWeChat.Models.QY;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.QY
{
    public interface ILazyQYContact
    {
        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="name">部门中文名称</param>
        /// <param name="name_en">部门英文名称</param>
        /// <param name="parentid">父部门id</param>
        /// <param name="order">在父部门中的次序值</param>
        /// <param name="id">部门id,指定时必须大于1,若不填该参数,将自动生成id</param>
        /// <returns></returns>
        Task<dynamic> CreateDeptAsync(string name, string name_en, int parentid, int order, int? id);

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="id">部门id</param>
        /// <param name="name">部门中文名称</param>
        /// <param name="name_en">部门英文名称</param>
        /// <param name="parentid">父部门id</param>
        /// <param name="order">在父部门中的次序值</param>
        /// <returns></returns>
        Task<dynamic> UpdateDeptAsync(int id, string name, string name_en, int parentid, int order);

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        Task<dynamic> DeleteDeptAsync(int id);

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="id">部门id.获取指定部门及其下的子部门(以及及子部门的子部门等等,递归). 如果不填,默认获取全量组织架构</param>
        /// <returns></returns>
        Task<dynamic> GetDeptsAsync(int? id);

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="tagid">标签id,非负整型,指定此参数时新增的标签会生成对应的标签id,不指定时则以目前最大的id自增.</param>
        /// <param name="tagname">标签名称</param>
        /// <returns></returns>
        Task<dynamic> CreateTagAsync(int? tagid, string tagname);

        /// <summary>
        /// 更新标签名字
        /// </summary>
        /// <param name="tagid"></param>
        /// <param name="tagname"></param>
        /// <returns></returns>
        Task<dynamic> UpdateTagAsync(int tagid, string tagname);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagid"></param>
        /// <returns></returns>
        Task<dynamic> DeleteTagAsync(int tagid);

        /// <summary>
        /// 获取标签成员
        /// </summary>
        /// <param name="tagid">标签ID</param>
        /// <returns></returns>
        Task<dynamic> GetTagUsersAsync(int tagid);

        /// <summary>
        /// 增加标签成员
        /// </summary>
        /// <param name="tagid">tagid</param>
        /// <param name="userlist">成员列表</param>
        /// <returns></returns>
        Task<dynamic> AddUsersforTagAsync(int tagid, params string[] userlist);

        /// <summary>
        /// 增加标签成员
        /// </summary>
        /// <param name="tagid">tagid</param>
        /// <param name="partylist">部门id列表</param>
        /// <returns></returns>
        Task<dynamic> AddUsersforTagAsync(int tagid, params int[] partylist);

        /// <summary>
        /// 删除标签成员
        /// </summary>
        /// <param name="tagid">tagid</param>
        /// <param name="userlist">成员列表</param>
        /// <returns></returns>
        Task<dynamic> DeleteUsersforTagAsync(int tagid, params string[] userlist);

        /// <summary>
        /// 删除标签成员
        /// </summary>
        /// <param name="tagid">tagid</param>
        /// <param name="partylist">部门id列表</param>
        /// <returns></returns>
        Task<dynamic> DeleteUsersforTagAsync(int tagid, params int[] partylist);

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetTagsAsync();

        /// <summary>
        /// 创建成员
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <returns></returns>
        Task<dynamic> CreateMemberAsync(MemberModel member);

        /// <summary>
        /// 读取成员
        /// </summary>
        /// <param name="userid">成员UserID</param>
        /// <returns></returns>
        Task<dynamic> GetMemberAsync(string userid);

        /// <summary>
        /// 更新成员
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <returns></returns>
        Task<dynamic> UpdateMemberAsync(MemberModel member);

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="userid">成员userid</param>
        /// <returns></returns>
        Task<dynamic> DeleteMemberAsync(string userid);

        Task<dynamic> BatchDeleteMemberAsync(params string[] useridlist);
    }
}
