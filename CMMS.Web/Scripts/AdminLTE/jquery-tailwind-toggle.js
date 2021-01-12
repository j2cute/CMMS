$(function () {
    // simple jQuery toggles using Tailwind
    $('[data-toggle="checkbox-toggle"]:not(.checkbox-toggle-tw)').each(function () {
        if ($(this).find('input[type="checkbox"]').length = 1) {
            Toggle(this, false);
        }
    });


    $('[data-toggle="checkbox-toggle"]').attr('tabindex', '0').on('keydown', function (e) {
        if (e.keyCode == 13 || e.keyCode == 32) {
            e.preventDefault();
            $(this).find('input').click();
        }
    });

});


function Toggle(el,isChecked) {
    var chBoxRounded = $(el).data('rounded');
    chBoxRounded = (chBoxRounded !== undefined) ? chBoxRounded : 'rounded-full';
    var chBoxHandleSize = $(el).data('handle-size');
    chBoxHandleSize = (chBoxHandleSize !== undefined) ? chBoxHandleSize : '20';
    var chBoxHandleColor = $(el).data('handle-color');
    chBoxHandleColor = (chBoxHandleColor !== undefined) ? chBoxHandleColor : 'bg-white';
    var chBoxOffColor = $(el).data('off-color');
    chBoxOffColor = (chBoxOffColor !== undefined) ? chBoxOffColor : 'bg-red-700';
    var chBoxOnColor = $(el).data('on-color');
    chBoxOnColor = (chBoxOnColor !== undefined) ? chBoxOnColor : 'bg-green-700';
    $(el)
        .attr('data-toggle', 'checkbox-toggle')
        .css({ 'width': (chBoxHandleSize * 2.5) + 6 + 'px', 'padding': '3px', 'transition': 'all .25s' })
        .addClass(chBoxRounded + ' ' + chBoxOffColor + ' inline-flex cursor-pointer align-middle')
        .append('<b class="' + chBoxHandleColor + ' ' + chBoxRounded + ' shadow" style="width: ' + chBoxHandleSize + 'px; height: ' + chBoxHandleSize + 'px; transition: all .25s" />')
        .find('input')
        .addClass('w-px h-px opacity-0 absolute')
        .attr('tabindex', '-1')
        .on('change', function () {
            debugger;
            var isCheckedThis = ($(el).find('input[type="checkbox"]')).prop('checked');

            //$('input:checkbox').each(function () {
            //    $(el).prop('checked', false);
            //    $(el).closest('label').removeClass(chBoxOnColor).addClass(chBoxOffColor).find('b').css('transform', '');

            //});
           
            if (isCheckedThis === true) {

                $(el).find('input[type="checkbox"]').prop('checked', true);
                $(el).closest('label').removeClass(chBoxOffColor).addClass(chBoxOnColor).find('b').css('transform', 'translate(' + chBoxHandleSize * 1.5 + 'px, 0)');
            }
            else {
                $(el).find('input[type="checkbox"]').prop('checked', false);
                $(el).closest('label').removeClass(chBoxOnColor).addClass(chBoxOffColor).find('b').css('transform', '');
            }

            //if ($(el).is(':disabled')) {
            //    $(el).closest('label').addClass('opacity-25 pointer-events-none');
            //} else {
            //    $(el).closest('label').removeClass('opacity-25 pointer-events-none');
            //}
        }).trigger('change');
}
