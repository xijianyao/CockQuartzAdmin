
$(function () {
    $("#selecedGroupName").children().each(function () {
        if ($(this).children("a").text() !== $("#JobGroupName").val()) {
            $(this).attr({ "class": "" });
        } else {
            $(this).attr({ "class": "active" });
        }
    });


    PageInitialise();

    $(".btnRun").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        loadingFunction();
        $.ajax({
            url: url.btnRun,
            type: 'POST',
            data: { id: $this.parent().parent().parent().parent().attr("data-jobid") },
            success: function (data) {
                alert(data.message);
                freshwindow();
                removeLoading();
            }, error: function () {
                removeLoading();
            }
        });
    });

    $(".btnStop").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        loadingFunction();
        $.ajax({
            url: url.btnStop,
            type: 'POST',
            data: { id: $this.parent().parent().parent().parent().attr("data-jobid") },
            success: function (data) {
                alert(data.message);
                freshwindow();
                removeLoading();
            }, error: function () { removeLoading();}
        });
    });

    $(".btnResume").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        loadingFunction();
        $.ajax({
            url: url.btnResume,
            type: 'POST',
            data: { id: $this.parent().parent().parent().parent().attr("data-jobid") },
            success: function (data) {
                alert(data.message);
                freshwindow();
                removeLoading();
            }, error: function () { removeLoading(); }
        });
    });

    $(".btnStart").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        loadingFunction();
        $.ajax({
            url: url.btnStart,
            type: 'POST',
            data: { id: $this.parent().parent().parent().parent().attr("data-jobid") },
            success: function (data) {
                alert(data.message);
                freshwindow();
                removeLoading();
            }, error: function () { removeLoading(); }
        });
    });

    $(".btnDelete").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        Alert.SubConfirm("确定要删除吗", "confirm", function () {
            loadingFunction();
            $.ajax({
                url: url.btnDelete,
                type: 'POST',
                data: { id: $this.parent().parent().parent().parent().attr("data-jobid") },
                success: function (data) {
                    alert(data.message);
                    freshwindow();
                    removeLoading();
                }, error: function () { removeLoading(); }
            })
        });
    });

    $(".btnEdit").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        Alert.SubConfirm(
            "请输入新的Cron表达式（修改成功后，该job会重新启动，新计划将生效）",
            "confirm",
            function () {
                loadingFunction();
                $.ajax({
                    url: url.btnEdit,
                    type: 'POST',
                    data: { id: $this.parent().parent().parent().parent().attr("data-jobid"), cron: $("#editcorn").val() },
                    success: function (data) {
                        alert(data.message);
                        freshwindow();
                        removeLoading();
                    }, error: function () { removeLoading(); }
                });
            }, function () { }, " <input type='text'  class='form-control' placeholder='请填写Cron表达式' id='editcorn'/>");

    });

    $(".btnEditExceptionEmail").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        Alert.SubConfirm(
            "请输入job错误发送的邮件，多个以分号分隔",
            "confirm",
            function () {
                loadingFunction();
                $.ajax({
                    url: url.btnEditExceptionEmail,
                    type: 'POST',
                    data: { id: $this.parent().parent().parent().parent().attr("data-jobid"), exceptionEmail: $("#editexceptionEmail").val() },
                    success: function (data) {
                        alert(data.message);
                        freshwindow();
                        removeLoading();
                    },error: function () { removeLoading(); }
                });
            }, function () { }, " <textarea type='text' class='form-control' rows='4' placeholder='请输入邮箱地址' id='editexceptionEmail'/></textarea>");


    });
    $(".btnEditRequestUrl").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        Alert.SubConfirm(
            "请输入job的RequestUrl",
            "confirm",
            function () {
                loadingFunction();
                $.ajax({
                    url: url.btnEditRequestUrl,
                    type: 'POST',
                    data: {
                        id: $this.parent().parent().parent().parent().attr("data-jobid"), requestUrl: $("#editrequestUrl").val()
                    },
                    success: function (data) {
                        alert(data.message);
                        freshwindow();
                        removeLoading();
                    }, error: function () { removeLoading(); }
                });
            }, function () { }, " <input type='text'  class='form-control' placeholder='请输入job的RequestUrl' id='editrequestUrl'/>");

    });

    $("#btnAddJob").on("click", function () {
        $("#JobModal").modal({ width: '600' });
    });

    $("#modalBtnAddJob").on("click", function () {
        var param = $("#CreateJobform").serializeJson();
        Requests.AsnyPost(url.btnAddJob, JSON.stringify(param), function (data) {
            if (data) {
                alert("添加成功");
                window.location.reload();
            }
            else {
                alert("添加失败");
                return false;
            }
        });
    });
});
var PageInitialise = function PageInitialise() {
    var pageCount = $("#pageCount").val();
    var param = { "groupName": $("#JobGroupName").val() };
    Page.Initialise("J_PagesPanel", pageCount, 1, url.jobList, param, "divlist");
}
function freshwindow() {
    var pageIndex = $("#J_PagesPanel").children("div").children(".laypage_current").eq(0).text()||1;
    var param = { "groupName": $("#JobGroupName").val() };
    Requests.Post(url.jobList, JSON.stringify(param), function (data) {
        $("#divlist").html(data);
        PageInitialise();
    });
}