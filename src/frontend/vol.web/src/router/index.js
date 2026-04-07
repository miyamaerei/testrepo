import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'
import viewgird from './viewGird'
import store from '../store/index'
import redirect from './redirect'
const routes = [
  {
    path: '/',
    name: 'Index',
    component: () => import('@/views/Index.vue'),
    redirect: '/home',
    children: [
      ...viewgird,
      ...redirect,
      {
        path: '/home',
        name: 'home',
        component: () => import('@/views/Home.vue')
      }, {
        path: '/UserInfo',
        name: 'UserInfo',
        component: () => import('@/views/sys/UserInfo.vue')
      },
      {
        path: '/sysMenu',
        name: 'sysMenu',
        component: () => import('@/views/sys/system/Sys_Menu.vue')
      }, {
        path: '/coder',
        name: 'coder',
        component: () => import('@/views/builder/coder.vue')
      },
      {
        path: '/formDraggable',  //表单设计
        name: 'formDraggable',
        component: () => import('@/views/formDraggable/formDraggable.vue')
      },
      {
        path: '/formSubmit',  //表单提交页面
        name: 'formSubmit',
        component: () => import('@/views/formDraggable/FormSubmit.vue'),
        meta:{
          keepAlive:false
        }
      },
      {
        path: '/formCollectionResultTree',  //显示收集的数据表单
        name: 'formCollectionResultTree',
        component: () => import('@/views/formDraggable/FormCollectionResultTree.vue'),
        meta:{
          keepAlive:false
        }
      },
      {
        path: '/signalR',  //消息推送
        name: 'signalR',
        component: () => import('@/views/signalR/Index.vue'),
        meta:{
          keepAlive:false
        }
      },
      {
        path: '/kanban',  //E-Kanban 执行看板
        name: 'kanban',
        redirect: '/kanban/index',
        meta: {
          title: 'E-Kanban',
          icon: 'el-icon-s-grid'
        },
        children: [
          {
            path: 'index',
            name: 'kanbanIndex',
            component: () => import('@/views/Kanban/Index.vue'),
            meta: { title: '执行看板', icon: 'el-icon-s-data' }
          },
          {
            path: 'project',
            name: 'projectList',
            component: () => import('@/views/Project/Index.vue'),
            meta: { title: '项目管理', icon: 'el-icon-folder' }
          },
          {
            path: 'manual',
            name: 'manualKanban',
            component: () => import('@/views/Kanban/ManualKanbanPage.vue'),
            meta: { title: '手动看板管理', icon: 'el-icon-edit' }
          },
          {
            path: 'detail/:id',
            name: 'kanbanDetail',
            component: () => import('@/views/Kanban/Detail.vue'),
            meta: { title: '卡片详情' }
          }
        ]
      }
    ]
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/Login.vue'),
    meta:{
        anonymous:true
      }
  }
]

const router = createRouter({
  history: createWebHashHistory(), //createWebHistory(process.env.BASE_URL),
  routes
})


router.beforeEach((to, from, next) => {
  if (to.matched.length == 0) return next({ path: '/404' });
  //2020.06.03增加路由切换时加载提示
  store.dispatch("onLoading", true);
  if ((to.hasOwnProperty('meta') && to.meta.anonymous) || store.getters.isLogin() || to.path == '/login') {
    return next();
  }

  next({ path: '/login', query: { redirect: Math.random() } });
})
router.afterEach((to, from) => {
  store.dispatch("onLoading", false);
})
router.onError((error) => {
  // const targetPath = router.currentRoute.value.matched;
  try {
      console.log(error.message);
    if (process.env.NODE_ENV == 'development') {
      //alert(error.message)
    }
    localStorage.setItem("route_error", error.message)
  } catch (e) {

  }
 // window.location.href = '/'
});
export default router
