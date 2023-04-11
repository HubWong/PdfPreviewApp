function operateFormatter(value, row, index) {
  return [
    '<div class="btn-group" role="group" aria-label="...">',
    '<button type="button" class="RoleOfedit btn btn-xs btn-primary">修改</button>',
    '<button type="button" class="RoleOfdelete btn btn-xs btn-danger" >删除</button>',
    '</div>'
  ].join('')
}
// 格式化类型
function typeFormatter(value, row, index) {
  if (value === 'menu') {
    return '菜单'
  }
  if (value === 'button') {
    return '按钮'
  }
  if (value === 'api') {
    return '接口'
  }
  return '-'
}
// 格式化状态
function statusFormatter(value, row, index) {

  if (value === 1) {
    return '<span class="label label-success">正常</span>'
  } else {
    return '<span class="label label-default">锁定</span>'
  }
}

/**
 * 选中父项时，同时选中子项
 * @param datas 所有的数据
 * @param row 当前数据
 * @param id id 字段名
 * @param pid 父id字段名
 */
function selectChilds(datas, row, id, pid, checked) {
  for (var i in datas) {
    if (datas[i][pid] == row[id]) {
      datas[i].check = checked
      selectChilds(datas, datas[i], id, pid, checked)
    }
  }
}

function selectParentChecked(datas, row, id, pid) {
  for (var i in datas) {
    if (datas[i][id] == row[pid]) {
      datas[i].check = true
      selectParentChecked(datas, datas[i], id, pid)
    }
  }
}

class TreeTable {
  constructor(tbId, data) {
    this.tbId = tbId
  }
  init() {
    let $table = $(this.tbId),
      _json = $('#treeData').text()
    let jd = eval('(' + _json + ')'),
      data2 = JSON.parse(jd)

    $table.bootstrapTable({
      data: data2,
      idField: 'id',
      dataType: 'jsonp',
      columns: [
        {
          field: 'check',
          checkbox: true,
          formatter: function (value, row, index) {
            if (row.check == true) {
              //设置选中
              return { checked: true }
            }
          }
        },
        { field: 'text', title: '名称' },
        //{ field: 'pid', title: 'pid' },
        { field: 'id', title: 'id', align: 'center' },
        {
          field: 'operate',
          title: '操作',
          align: 'center',
          events: operateEvents,
          formatter: 'operateFormatter'
        }
      ],
      // bootstrap-table-treegrid.js 插件配置 -- start
      //在哪一列展开树形
      treeShowField: 'text',
      //指定父id列
      parentIdField: 'pid',

      onResetView: function (data) {
        //console.log('load');
        $table.treegrid({
          initialState: 'collapsed', // 所有节点都折叠 or 'expanded'      
          treeColumn: 1,
          //expanderExpandedClass: 'glyphicon glyphicon-minus', //图标样式
          //expanderCollapsedClass: 'glyphicon glyphicon-plus',
          onChange: function () {
            $table.bootstrapTable('resetWidth')
          }
        })

        //只展开树形的第一级节点
        $table.treegrid('getRootNodes').treegrid('expand')
      },
      onCheck: function (row) {
        var datas = $table.bootstrapTable('getData')
        // 勾选子类
        selectChilds(datas, row, 'id', 'pid', true)

        // 勾选父类
        selectParentChecked(datas, row, 'id', 'pid')

        // 刷新数据
        $table.bootstrapTable('load', datas)
      },

      onUncheck: function (row) {
        var datas = $table.bootstrapTable('getData')
        selectChilds(datas, row, 'id', 'pid', false)
        $table.bootstrapTable('load', datas)
      }
      // bootstrap-table-treetreegrid.js 插件配置 -- end
    })
  }
}

//初始化操作按钮的方法
window.operateEvents = {
  // 'click .RoleOfadd': function (e, value, row, index) {
  //   TreeTable.add(row.id)
  // },
  'click .RoleOfdelete': function (e, value, row, index) {
    menuController.columnDel(row.id, e.target)
  },
  'click .RoleOfedit': function (e, value, row, index) {
    menuController.columnEditAdd(row.id)
  }
}
