var AppHost = 'http://localhost:5002',
    GetPagedList = function (pg, ttl, tp) {
        console.log(pg)
    };

class HomeIndex {
    static Cur_bindid = -1;
    static cls_catch = 'catch';

    constructor() {
        this.api_pdfDefaultList = AppHost + '/home/default';
        this.api_pdfList = AppHost + '/home/_PdfListView';      
        this.detail_div = $('.row>div');
        this.bdModel = new BindingModel('homeIndex');
    }
    _bindEvts() {
        let _slf = this;
        $('.sec_1 div.bd>ul>li>a').unbind('click')
            .on('click', function () {

                $(this).parent()
                    .addClass(HomeIndex.cls_catch)
                    .siblings().removeClass(HomeIndex.cls_catch);

                let obj = $('li.catch').find('a');
                for (let index = 0; index < obj.length; index++) {
                    const element = obj[index];
                    _slf.bdModel.createModel($(element).attr('rel'))
                }
                _slf.bdModel.clk = $(this).attr('rel');
                _slf._loadList();
            })
    }
    _fill_bd_data(k, list) {
        let initDom = '';
        switch (k) {
            case 'mo_kuai':
                $('div.mk ul').empty();
                for (let index = 0; index < list.length; index++) {
                    const data = list[index], li = DomGen.createDom('li');

                    initDom = DomGen.createDom('a',
                        { 'rel': 'mk_' + data.id, 'href': 'javascript:;' },
                        data.title);
                    li.append(initDom);
                    $('div.mk ul').append(li);
                }

                break;
            case 'xue_ke':
                $('div.xk ul').empty();
                for (let index = 0; index < list.length; index++) {
                    const data = list[index], li = DomGen.createDom('li');

                    initDom = DomGen.createDom('a',
                        { 'rel': 'xk_' + data.id, 'href': 'javascript:;' },
                        data.title);
                    li.append(initDom);
                    $('div.xk ul').append(li);
                }
                break;
            case 'ban_ben':
                $('div.bb ul').empty();
                for (let index = 0; index < list.length; index++) {
                    const data = list[index], li = DomGen.createDom('li');

                    initDom = DomGen.createDom('a',
                        { 'rel': 'bb_' + data.id, 'href': 'javascript:;' },
                        data.title);
                    li.append(initDom);
                    $('div.bb ul').append(li);
                }
                break;
            default:
                break;
        }
    }

    _viewPdfList(doms) {
        let _slf = this,
            jevl = function (d) {
                return eval('(' + d + ')')
            },
            _refreshRest = function (d) {  //rest level data recreate.
                for (const key in d) {
                    if (Object.hasOwnProperty.call(d, key)) {
                        const element = d[key];
                        _slf._fill_bd_data(key, element);
                    }
                }

                $('#bd section:gt(' + _slf.bdModel.clkIndex + ')')
                    .each(function () {
                        $(this).find("li").eq(0).addClass(HomeIndex.cls_catch);
                    });
                _slf._bindEvts();
            };

        $('#sec_2 .card-body').html(doms);
        let data = $('#sec_2 .card-body').find('script').text(), jd;
        if (data) {
            jd = JSON.parse(jevl(data));
            _refreshRest(jd);
        }
    }

    _loadList() {
        if (this.bdModel.isValid()) {
            ToolFunc.doAjaxFn(this.api_pdfDefaultList, this.bdModel, v => this._viewPdfList(v), 'post')
        } else {
            console.log('bindmodel is not valid')
        }
    }
    start() {
        $('section').each(function () {
            let liDom = $(this).find("div.bd>ul").find('li');
            if (liDom.length !== 0) {
                $(this).find('li').eq(0).addClass(HomeIndex.cls_catch)
            } else {
                $(this).find("div.bd>ul").html(BindingModel.emptyDataLi())
            }

        })
        this._bindEvts();
    }
}


let index = new HomeIndex();
index.start();
