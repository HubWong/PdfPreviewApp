//sidebar controller in the index page of dashboard.
class MenuController extends BasicController {
  static cur_row_cls = 'cur'
  static sidebarEleId = 'sidebar_div'
  static cur_id = -1 //record id in adding or editing page

  constructor(mainId = '') {
    super()
    this.tools = new ToolFunc(MAIN_DIV)
    this.newClsForRow = 'ckd'
    this.pdf_urls = this.httpAction.pdf_urls
    this.prevRoute = '/'
  }

  back = () => {
    let _slf = this,
      routes = BaseReqModel.Routes

    if (routes.length > 0) {
      routes.pop()
      this.prevRoute = routes[routes.length - 1]
      if (!this.prevRoute) {
        return
      }
      try {
        let d = this.prevRoute,
          data = JSON.parse(d.data)

        if (d) {
          $('a.link-dark').each(function () {
            if ($(this).text() == data.menu) {
              this.setNavActive(data.menu)
              this.CurrentSidebar = activSidebar()
              _slf.highlightNav()
              return
            }
          })
          this.requestView(d.url, data)
        }
      } catch (error) {
        console.log('[error]:' + error)
      }
    } else {
      return
    }
  }

  _getApi = t => {
    let default_rt = ''
    switch (t) {
      case '数据关联':
        default_rt = this.pdf_urls.get_bdingV
        break
      case '电子书列表':
        default_rt = this.pdf_urls.get_booksV
        break
      default:
        default_rt = this.pdf_urls.get_categsV
        break
    }
    return default_rt
  }

  start() {
    if (this.CurrentSidebar.title) {
      this.CurrentSidebar.routeTitle = this._getApi(this.CurrentSidebar.title)
      this.prevRoute = Route.genRoute(this.CurrentSidebar)
    } else {
      throw Error('sidebar title not found.')
    }

    this.request(this.CurrentSidebar.jqEle)
  }

  _getQsData() {
    return new PagedModel(this.CurrentSidebar.title, 'orderno');
  }

  request = eleLi => { //dashboard
    let txt = $(eleLi).attr('data-title')
    this.setNavActive(txt)
    let r = this._getApi(this.CurrentSidebar.title),
      pgMdl = this._getQsData();
    this.requestView(r, pgMdl, pgMdl)
  }


  showOperResult(isOk, msg, cls) {
    let obj = new OperResultDialog(isOk, msg, cls)
    obj.popUp()
  }

  rowEdit = eleTr => {
    //edit view  

    this.tableFunc(
      'edit',
      $(eleTr)
        .parents('tr')
        .find('.id')
        .text()
    )
  }

  rowDel = eleTr => {
    this.tableFunc(
      'dels',
      $(eleTr)
        .parents('tr')
        .find('.id')
        .text()
    )
  }

  selectRow = eleTr => {
    let cur = MenuController.cur_row_cls
    $(eleTr)
      .addClass(cur)
      .siblings()
      .removeClass(cur)
  }

  /**
   *
   * @param {*} data
   * @param {single row or multiple row } type
   */
  _delRows(data, type = 's') {
    let _url
    this.CurrentSidebar = this.activeSidebar;

    if (this.CurrentSidebar.title.indexOf('电子书') !== -1) {
      _url = this.pdf_urls.api_pdf
    } else if (this.CurrentSidebar.title.indexOf('管理') !== -1) {
      _url = this.pdf_urls.api_itemCateg
    }

    ToolFunc.doAjaxFn(
      _url,
      data,
      r => {
        if (r > 0) {
          if (type == 'm') {
            ToolFunc.remove_ckd_tr('table')
          } else if (type == 's') {
            ToolFunc.remove_ckd_tr('table', 'cur')
          }

          this.showOperResult(
            true,
            '删除了' + r + '条记录',
            OperResultDialog.ENV_Class.info
          )
        } else {
          this.showOperResult(
            false,
            '删除失败',
            OperResultDialog.ENV_Class.danger
          )
        }
      },
      'delete'
    )
  }

  /**
   * bind form submit event
   * @param {ajax url} _url
   */
  static formEvt = function (_url) {
    let forms = document.getElementsByTagName('form'),
      _isAdd = 0

    $(forms).each(function () {
      let f = this
      $(f).on('submit', function (e) {
        e.preventDefault()
        _isAdd = $(f)
          .find('.IsAdd')
          .val()
        if (!_isAdd) {
          return
        }
        if (_url) {
          HttpAction.formSubmit(_url, $(f).attr('id')).then(d => {
            let dig = new OperResultDialog()
            if (d.data && d.data !== 0) {
              let msg = '增加了1条记录'
              if (_isAdd !== '1') {
                msg = '更新了1条记录'
              } else {
                let idInput = $('input[name=id]')

                if (idInput.length > 0) {
                  $('input[name=id]').val(d.data)
                  MenuController.cur_id = d.data
                } else {
                  console.log('[*]:id dom not found')
                }
              }
              dig.isOk = true
              dig.msg = msg
              dig.cls = OperResultDialog.ENV_Class.success
            } else {
              dig.isOk = false
              dig.msg = '保存失败'
              dig.cls = OperResultDialog.ENV_Class.danger
            }
            dig.popUp()
          })
        } else {
          console.log('[*error*]:url is none when form submits')
        }
      })
    })
  }

  tableFunc = (type, data) => {
    //if btn of 'add' click , data is undefined.

    MenuController.cur_id = data || -1
    let _slf = this,
      _fmCbUrl = ''
    switch (type) {
      case 'dels':
        let trDom = $('table tbody').find('tr')
        if (trDom.length == 0) {
          console.log('is 0')
          return
        }
        if (data == -1) {
          //is multiple deleting $("input[name='checkbox']:checkbox:checked")
          let ids = $('table tbody [type=checkbox]:checked'),
            id_array = []
          for (let index = 0; index < ids.length; index++) {
            const element = $(ids)[index]
            let id = $(element)
              .parents('tr')
              .find('[class=id]')
              .text()
            id_array.push(id)
          }
          if (id_array.length > 0)
            OperResultDialog.confirm(
              '是否删除这' + id_array.length + '条及相关数据关联?',
              () => {
                _slf._delRows(id_array, 'm')
              }
            )
          else {
            _slf.showOperResult(
              false,
              '请选择一条记录',
              OperResultDialog.ENV_Class.warning
            )
          }
        } else {
          OperResultDialog.confirm('是否删除本条及相关联记录?', () => {
            _slf._delRows(data)
          })
        }
        break

      case 'add': //add new record for different table
        _fmCbUrl = this.pdf_urls.api_itemCateg
        let _url = ''
        if (this.CurrentSidebar.title.indexOf('电子书') != -1) {
          _url = this.pdf_urls.get_pdfNewEditV + '/' + $('[name=bindid]').val()
          _fmCbUrl = this.pdf_urls.api_pdf
          if ($('[name=bindid]').val()) {
            this.requestView(
              _url,
              null,
              new FmModel('add_edit', _fmCbUrl, null, $('[name=bindid]').val())
            )
          } else {
            //bindid is undefined.
          }
          break
        } else if (this.CurrentSidebar.title.indexOf('管理')) {
          //categories
          _url = this.pdf_urls.get_tableAddEditV
          this.requestView(
            _url,
            null,
            new FmModel('add_edit', _fmCbUrl, null, null)
          )
          break
        }

      case 'edit': //add new itemcategory
        if (this.CurrentSidebar.title.indexOf('电子书') != -1) {
          let _url =
            this.pdf_urls.get_pdfNewEditV +
            '/' +
            $('[name=bindid]').val() +
            '/' +
            data,
            _fmCbUrl = this.pdf_urls.api_pdf

          this.requestView(
            _url,
            null,
            new FmModel('add_edit', _fmCbUrl, data, null)
          )
          break
        } else if (this.CurrentSidebar.title.indexOf('管理')) {
          _fmCbUrl = this.pdf_urls.api_itemCateg + '/update'
          this.requestView(
            this.pdf_urls.get_tableAddEditV,
            { id: data },
            new FmModel('add_edit', _fmCbUrl, data, null)
          )
          break
        }
        break
      default:
        break
    }
  }

  static viewCb = data => {
    if (!data) {
      return
    }
    let sidebar = data.menu

    try {
      if (sidebar) {
        switch (sidebar) {
          case '数据关联':
            new DataBindPage('数据关联').init()
            MenuController.rstTopBtns({ ok: true }, { ok: false }, { ok: false })
            break
          case '电子书列表':
            new PdfPage().init()
            MenuController.rstTopBtns({ ok: true }, { ok: true }, { ok: true })
            break
          case 'add_edit':
            
            this.formEvt(data.submitUrl)
            MenuController.rstTopBtns({ ok: true }, { ok: false }, { ok: true })
            console.log(data.submitUrl)
            if(data.submitUrl.indexOf('pdf')!==-1){
              FmModel.ueInit()
            }
           
           
            break
          default:
            MenuController.rstTopBtns({ ok: true }, { ok: true }, { ok: true })
            ToolFunc.add_cls_tr_when_click('table', this.newClsForRow)

            return
        }
      }
    } catch (error) {
      console.log(error)
    }
  }
}
let
  menuController = new MenuController(MAIN_DIV);
menuController.start();
