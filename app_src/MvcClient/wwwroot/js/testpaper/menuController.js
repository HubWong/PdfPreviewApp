/**
 * *********************
 * testpaper dashboard contrl
 * ************************
 */

BaseReqModel.Routes = [
  new Route(AppHost + '/testpaper', new FmModel('创建栏目'))
]

var treeNodeArray = []

//testpaper menu controller

class MenuController extends BasicController {
  constructor() {
    super()
    this.testpprApi = this.httpAction.tstppr_urls;
  }

  /**
   * reset all sidebar urls with api url object above.
   */
  _resetUrls() {
    this.navBars.forEach((s, y) => {
      s.resetRoute(this.testpprApi)
    })
  }

  request(ele_a) {  //testpaper
    let txt = $(ele_a).attr('data-title'),
      slf = this;
    slf.setNavActive(txt);
    let reqData = slf._getQsData();
    slf.requestView(reqData.ajaxUrl, reqData.reqModel, reqData.reqModel, this.viewCb)
  }

  addProp = function (e) {
    let qsData = this._getQsData('editView', false)
    this.requestView(qsData.ajaxUrl, qsData.reqModel, null, () => {
      return MyForm.formEvt(this.testpprApi.属性.api)
    })
  }

  delProp = function (e) {
    let id = $(e)
      .parents('tr')
      .find('td.id')
      .text();
    OperResultDialog.confirm('删除此条数据?', () => {
      this.httpAction.request_data(
        this.testpprApi.属性.api + '/' + id,
        null,
        r => {
          if (r) {
            $(e)
              .parents('tr')
              .hide()
            new OperResultDialog(true, '已删除').popUp()
          }
        },
        'delete'
      )
    })
  }
  selectRow(dom) {
    console.log('row selected', dom)
  }
  rowEdit(ele) {
    console.log(ele)
  }
  rowDel(ele) {
    console.log(ele)
  }
  /**
   *
   * @returns request data.
   */
  _getQsData(key, isPage = true) {
    let
      orderBy = 'makeday',
      slf = this,
      menu = this.CurrentSidebar.title,
      reqM = new ReqViewModel(MAIN_DIV),
      _getUrl = function (urlkey) {
        let uris = {};
        if (menu == '年份' || menu == '省份') {
          uris = slf.testpprApi['属性']
        }
        else {
          uris = slf.testpprApi[menu]
        }
        if (key) {
          urlkey = key;
        }
        return uris[urlkey];
      };

    switch (menu) {
      case '省份':
        reqM.ajaxUrl = _getUrl('defaultViewSf')
        if (isPage) reqM.reqModel = new PagedModel(menu, orderBy)
        else reqM.reqModel = new FmModel(menu, this.testpprApi.属性.api, null, null)
        break
      case '年份':
        reqM.ajaxUrl = _getUrl('defaultViewNf')
        if (isPage) reqM.reqModel = new PagedModel(menu, orderBy)
        else reqM.reqModel = new FmModel(menu, this.testpprApi.属性.api, null)
        break
      case '栏目列表':
        reqM.ajaxUrl = _getUrl('defaultView')
        reqM.reqModel = new PagedModel(menu, orderBy)
        break
      case '试卷列表':
        reqM.ajaxUrl = _getUrl('listView')
        reqM.reqModel = new PagedModel(menu, orderBy)
        break
      case '上传试卷':
        reqM.ajaxUrl = _getUrl('defaultView')
        reqM.reqModel = new FmModel(menu, this.testpprApi.上传试卷.api, null, null, this)
        break
      default:
        reqM.ajaxUrl = _getUrl('defaultView')
        reqM.reqModel = new FmModel(menu, this.testpprApi.创建栏目.api)
    }
    return reqM
  }
  /**
   * button on the top or in the table click event
   * @param {event name} type :del and add
   * @param {ele} data
   */
  tableFunc = (type, data) => {
    let menu_title = this.CurrentSidebar.title;
    switch (menu_title) {
      case '栏目列表':
        if (type == 'add') {
          this.columnEditAdd()
        }
        break
      case '省份':
      case '年份':
        if (type === 'add') {
          let qsData = this._getQsData('editView', false)
          this.requestView(qsData.ajaxUrl, qsData.reqModel, null, () => {
            return MyForm.formEvt(this.testpprApi.属性.api)
          })
        } else if (type == 'dels') {

        }
        break
      case '上传试卷':
        break
      default:
        //创建栏目
        if (type == 'add') {
          console.log('clm add')
        } else if (type == 'dels') {
          console.log('clm del')
        }
        break
    }
  }
  back = () => {
    let _slf = this,
      routes = BaseReqModel.Routes,
      prev_title = '创建栏目',
      ele_a,
      data = {},
      nav_a = $('#home-collapse').find('a.link-dark')
    if (routes.length > 1) {
      routes.pop()
    }

    _slf.prevRoute = routes[routes.length - 1]
    data = JSON.parse(_slf.prevRoute.data)

    try {
      if (_slf.prevRoute) {
        prev_title = data.menu
      }

      $(nav_a).each(function () {
        if (
          $(this)
            .text()
            .trim() == prev_title
        ) {
          ele_a = this
        }
      })

      if (ele_a && $(ele_a).length > 0) {
        $(ele_a).trigger('click')
      } else {
        console.log('ele not found')
      }
    } catch (error) {
      console.log(error)
    }
  }
  _reset_layout() {
    let h = $(document).height(),
      w = $(document).width(),
      nh = $('.navbar')
        .css('margin-bottom', 0)
        .height(),
      r = parseInt(h) - nh - 40
    $('div.d-flex').css('line-height', '45px')
    $('div.container-fluid')
      .css('padding-left', 0)
      .css('padding-right', 0)
    $('div.container-fluid>.row').css('margin', '0')
    $('#sidebar_module').addClass('left')
    $('#main_rgt').addClass('right')
    $('#sidebar_module,#main_rgt')
      .css('padding-top', '1.2em')
      .height(r)
  }
  start() {
    this._resetUrls()
    this._reset_layout()
    this.request(this.CurrentSidebar.jqEle)
  }

  /**
   * column table clicked
   * @param {clicked id} id
   */
  columnEditAdd(id) {
    if (id) {
      this.requestView(this.testpprApi.创建栏目.defaultView
        , new FmModel('创建栏目', this.testpprApi.创建栏目.api, null, null)
        , null, this.viewCb)
    } else {
      $('.nav a')
        .eq(0)
        .trigger('click')
    }
  }

  columnDel(id, target) {
    OperResultDialog.confirm('删除记录?', () => {
      this.httpAction.request_data(
        this.testpprApi.栏目列表.api + '/' + id,
        null,
        r => {
          if (r == 1) {
            new OperResultDialog(true, '已删除').popUp()
            $(target)
              .parents('tr')
              .hide()
              .remove()
          }
        },
        'delete'
      )
    })
  }

  /**
   * create tree by ele
   * @param {dom of tree} ele
   */
  static treeDropdown(ele) {
    if (ele) {
      let tres = new DataTree(ele);
      // if there is tree dom show the tree panel.
      try {
        let str_data = $('#data_tree').text()
        str_data = eval('(' + str_data + ')')
        treeNodeArray = JSON.parse(str_data)
        tres.treeData = treeNodeArray;
        tres.init();
      } catch (error) {
        console.log(error)
      }
    }
  }

  static setClmPrpty(nf = 0, sf = 0) {
    $('div.col-md-10>a').unbind('click').on('click', function () {
      $(this).siblings().removeClass('active');
      $(this).addClass("active");
    })
    if (typeof (nf) == "number") {
      $('div.col-md-10.nf').find('a').eq(nf).addClass("active")
    }
    if (typeof (sf) == "number") {
      $('div.col-md-10.sf').find('a').eq(sf).addClass("active")
    }
  }
  viewCb(data) {
    if (data && data.menu) {
      let sidebar = data.menu
      switch (sidebar) {
        case '栏目列表':
          MenuController.rstTopBtns({ ok: true }, { ok: true }, { ok: true })
          let tb = document.getElementById('td'),
            treeTbl = new TreeTable(tb);
          treeTbl.init()
          break
        case '省份': //this page is lst page.
        case '年份':
          MenuController.rstTopBtns({ ok: true }, { ok: true }, { ok: true })
          break
        case '试卷列表':
          break
        case '上传试卷':
          MenuController.treeDropdown(document.getElementById('collapseOne'))
          MenuController.rstTopBtns({ ok: true }, { ok: false }, { ok: true })
          MenuController.setClmPrpty()
          let prevData = $('#hideInfo'),
            jqUp = new JqUploader(null, 'tpUploader', '上传试卷', 1, 'doc,docx');
          jqUp.init({
            autoSubmit: true,
            showDelete: true,
            deleteStr: "del",
            onLoad: function (obj) {
              if (jqUp.id !== null) {
                obj.createProgress(prevData.attr("data-name"), prevData.attr('data-path'), prevData.attr('data-size'))
              }
            }
          })

          break
        default:
          //创建栏目          
          MenuController.treeDropdown(document.getElementById('collapseOne'))
          MenuController.rstTopBtns({ ok: true }, { ok: false }, { ok: false })
          break
      }
    }
  }
}

let menuController = new MenuController()
menuController.start()
