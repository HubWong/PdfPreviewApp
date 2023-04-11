class BasicController {

    static starting = true;// is inital for functions from outside.

    constructor(mainId = MAIN_DIV) {
        this._navArray = []
        this.collectModules();

        if (this._navArray.length == 0) {
            throw Error("nav menu is null");
        }
        this.cls_cur_nav = 'cur_btn';
        this._curNav = this.menuModules[0].SidebarArray[0];
        this.httpAction = new HttpAction(mainId);//get all sidebar urls of  api   
        this._navBars = [];
        this.layoutSetups();
    }
    layoutSetups() {
        $('li.nav').on('click', function () {
            $(this).siblings().find("a").removeClass('cur_btn');
            $(this).find('a.link-dark').addClass('cur_btn')
            $(this).parents('div.mb-1').find('a.link-dark').removeClass('cur_btn')
        })
    }

    get CurrentSidebar() {
        return this._curNav
    }

    set CurrentSidebar(nav) {
        if (nav) {
            this._curNav = nav;
        } else {
            this._curNav = this._navArray[0].SidebarArray[0]
        }
    }

    /**
 * request view by data model of sidebar type
 * @param {url without qstr} route
 * @param {qs model object} data
 * @param { form data object } rstParam
 * @returns
 */
    requestView = (ajxUrl, qsData, rstParam, cb) => {
        this.httpAction.request_view(
            ajxUrl,
            qsData,
            cb || MenuController.viewCb,
            rstParam,
            'GET'
        )
    }

    setNavActive(txt) {
        if (this.navBars.length === 0) {
            throw Error('nav bar array is empty')
        }

        $.each(this.navBars, (i, b) => {
            b.active = false;
        })

        $.each(this.navBars, (i, b) => {
            if (b.title === txt) {
                b.active = true;
                this._curNav = b;
                return;
            }
        })
    }

    get navBars() {
        let sf = this;
        $.each(this._navArray, function (i, d) {
            $.each(d.SidebarArray, (_i, _d) => {
                let h = sf._navBars.findIndex((value, index, array) => {
                    return value.title === _d.title
                })

                if (h === -1)
                    sf._navBars.push(_d)
            })

        })

        return this._navBars;
    }

    collectModules(jqEle = '.accordion-item') {
        let module_div = $(jqEle), _x = [];

        $.each(module_div, function (i, d) {
            let md = new MenuModuleModel(i, d)
            _x.push(md)
        })
        this._navArray = _x;
    }

    get menuModules() {
        return this._navArray
    }

    checkAll() {
        ToolFunc.checkAll('table')
    }


    /**
   * make sm btns disabled or abled.
   * @param {kv back btn is abled} bck
   * @param {*} delete button element
   * @param {add new button} add
   */
    static rstTopBtns(bck, dels, add) {
        let btns = $('#btnTop button')
        for (let index = 0; index < btns.length; index++) {
            const element = btns[index],
                txt = $(element).text()
            if (txt.indexOf('返回') !== -1) {
                ToolFunc.disableEle(element, bck.ok)
            } else if (txt.indexOf('删除') !== -1) {
                ToolFunc.disableEle(element, dels.ok)
            } else if (txt.indexOf('添加') !== -1) {
                ToolFunc.disableEle(element, add.ok)
            }
        }
    }
}